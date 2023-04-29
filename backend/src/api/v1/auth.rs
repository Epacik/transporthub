use actix_web::{post, web, Result, HttpResponse, HttpRequest, dev::HttpServiceFactory};
use rand::distributions::DistString;
use rand_distr::Alphanumeric;
use rbatis::{executor::Executor, rbdc::datetime::DateTime, Rbatis};
use serde::{Deserialize, Serialize};
use crate::{db_model::{ context, User, UserType, PasswordVerificationResult, UserAccessKeys }, errors::{self, ErrorResponse}};

use super::{UserInfo, Response};

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/auth")
    .service(authenticate)
    .service(refresh_token)
    .service(logout)
}


#[derive(Deserialize)]
pub struct LoginRequest {
    pub user: String,
    pub password: String,
    //pub device_id: String, // change that to IP later (remember about cloudflare)
    pub disconnect_other_sessions: Option<bool>,
}

#[derive(Serialize)]
pub struct LoginResponse {
    pub user: String,
    pub key: String,
}

impl LoginResponse {
    fn new(user: String, key: String) -> LoginResponse {
        LoginResponse {
            user,
            key,
        }
    }
}

#[post("/login")]
pub async fn authenticate(body: web::Json<LoginRequest>, req: HttpRequest) -> Response {
    let mut context = context().await;
    let ip = match super::get_ip_address(&req) {
        Err(_) => return Err(errors::ip_unobtainable()),
        Ok(val) => val
    };

    log::trace!("Checking number of users");
    let count = User::count(&context).await.unwrap();

    if count == 0 {
        log::info!("This database contains no users, creating an `admin` user");

        User::create(&mut context, "admin", None, "admin", None, UserType::Admin, true, false).await;
    }


    let user = &body.user;
    log::info!("Looking for `{}`", user);
    let user = match User::select_by_name(&mut context, &user).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };


    let user = match user {
        None => return Err(errors::auth::invalid_username()),
        Some(u) => u,
    };


    match user.check_password(&body.password).await {
        PasswordVerificationResult::InvalidPassword => Err(errors::auth::invalid_password()),
        PasswordVerificationResult::Expired => Err(errors::auth::password_expired()),
        PasswordVerificationResult::Authorized => login_user(&mut context, &user, &body.disconnect_other_sessions, &ip).await,
    }
}

async fn login_user(context: &mut dyn Executor, user: &User, disconnect_others: &Option<bool>, device_id: &String) -> Response {
    let disconnect_others = disconnect_others.unwrap_or(false);

    // is user already logged in?
    if !user.multi_login {
        log::trace!("User is not allowed to have multiple sessions active");
        let access_keys = match UserAccessKeys::select_all_by_user(context, user.id.unwrap_or_default()).await {
            Ok(keys) => keys,
            Err(err) => return Err(errors::database_error(&err)),
        };

        let already_logged_in = access_keys.len() >= 1;

        if already_logged_in && !disconnect_others {
            log::info!("User is already logged in");
            return Err(errors::auth::multi_login_not_allowed());
        }

        if already_logged_in && disconnect_others {
            log::info!("Disconnecting sessions");
            let ids = access_keys.iter()
                .filter(|x| x.id.is_some())
                .map(|x| x.id.unwrap_or_default())
                .collect::<Vec<i32>>();

            let result = UserAccessKeys::delete_by_column_batch(context, "id", &ids).await;
            if result.is_err() {
                let err = result.unwrap_err();
                return Err(errors::database_error(&err));
            }
        }

    }

    // find a key that's not in use
    log::info!("Generating access key");
    log::trace!("Looking up all active access keys");
    let all_keys = match UserAccessKeys::select_all(context).await {
        Ok(val) => val.iter().map(|x| x.key.clone()).collect::<Vec<String>>(),
        Err(err) => return Err(errors::database_error(&err)),
    };
    let get_key = || Alphanumeric.sample_string(&mut rand::thread_rng(), 64);
    let mut key = get_key();

    while all_keys.contains(&key){
        key = get_key();
    }
    log::trace!("Generated key {key}");


    if let Err(err) = UserAccessKeys::create(context, &key, user.id.unwrap(), device_id).await {
        return Err(errors::database_error(&err));
    }

    let hash_id = super::get_hashid().encode(&[user.id.unwrap() as u64]);

    Ok(HttpResponse::Ok().json(&LoginResponse::new(hash_id, key)))
}



async fn get_access_key(context: &mut Rbatis, user_info: &UserInfo) -> Result<UserAccessKeys, ErrorResponse> {
    let access_key = match UserAccessKeys::select_by_key( context, &user_info.access_key()).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(value) => value,
    };

    let access_key = match access_key {
        None => return Err(errors::auth::invalid_token()),
        Some(s) => s,
    };

    if access_key.user_id != (user_info.user_id() as i32) || access_key.device_id != user_info.ip() {
        return Err(errors::auth::invalid_token());
    }

    Ok(access_key)
}

#[post("/refresh-token")]
pub async fn refresh_token(req: HttpRequest) -> Response {

    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let mut context = context().await;
    let mut access_key = match get_access_key(&mut context, &user_info).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    access_key.last_refreshed = DateTime::utc();

    match UserAccessKeys::update_by_column(&mut context, &access_key, "id").await {
        Ok(_) => Ok(HttpResponse::Ok().body("")),
        Err(err) => Err(errors::database_error(&err)),
    }

}

#[post("/logout")]
pub async fn logout(req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let mut context = context().await;

    let access_key: UserAccessKeys = match get_access_key(&mut context, &user_info).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };


    if let Some(id) = access_key.id {
        if let Err(err) = UserAccessKeys::delete(&mut context, id).await {
            return Err(errors::database_error(&err));
        }
    }


    Ok(HttpResponse::Ok().body("logged out"))

}
