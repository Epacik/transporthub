use std::time::Duration;
use actix_web::http::header::HeaderMap;

use actix_web::{
    HttpResponse,
    get,
    Responder,
    HttpRequest,
    dev::HttpServiceFactory,
};
use fastdate::DurationFrom;
use hash_ids::HashIds;
use rbatis::rbdc::datetime::DateTime;
use base64::{Engine as _, engine::general_purpose};

use crate::db_model::User;
use crate::errors;
use crate::{config, db_model::UserAccessKeys};

pub mod users;
pub mod auth;
mod middleware;
mod dto;

type Response = Result<HttpResponse, errors::ErrorResponse>;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/v1")
    .wrap(middleware::Auth::default())
    .wrap(actix_web::middleware::DefaultHeaders::new().add(("X-Version", "0.2")))
    .service(check_connection)
    .service(auth::scope())
    .service(users::scope())
}


#[get("/check-connection")]
async fn check_connection() -> impl Responder {
    HttpResponse::Ok().body("Connected")
}

fn get_hashid1() -> HashIds {
    let config = config::config();
    hash_ids::HashIds::builder()
        .with_salt(config.hash_ids_secret())
        .with_min_length(15)
        .finish()
}

fn decode_id<S: AsRef<str>>(string: &S) -> i32 {
    let hashids = get_hashid1();
    let id = hashids.decode(string.as_ref());

    if id.len()>= 1 { id[0] as i32 } else { 0 }
}

fn encode_id(id: i32) -> String {
    let hashids = get_hashid1();

    hashids.encode(&[id as u64])
}


async fn cleanup_stale_sessions(context: &mut dyn rbatis::executor::Executor) -> Result<(), rbatis::rbdc::Error> {
    let date = DateTime::utc() - Duration::from_minute(5);
    let stale_sessions = UserAccessKeys::select_older_than(context, date).await?;

    if stale_sessions.len() == 0 {
        return Ok(());
    }

    let ids = stale_sessions.iter()
        .map(|x| x.id.unwrap_or_default())
        .collect::<Vec<i32>>();

    UserAccessKeys::delete_by_column_batch(context, "id", &ids[..]).await?;

    Ok(())
}

fn get_auth_header_info(headers: &HeaderMap) -> Result<(i32, String), errors::ErrorResponse> {
    let auth_header = match headers.get("authorization") {
        Some(h) => h,
        None => return Err(errors::auth::invalid_token())
    };

    let token = match auth_header.to_str() {
        Ok(t) => t,
        Err(err) => return Err(errors::auth::invalid_token_info(&err)),
    };

    let token = token.split(" ").collect::<Vec<&str>>();

    if token.len() != 2 {
        return Err(errors::auth::invalid_token());
    }

    let token = token[1];

    let token = match general_purpose::STANDARD.decode(token) {
        Ok(val) => val,
        Err(err) => return Err(errors::auth::invalid_token_info(&err)),
    };

    let token = match std::str::from_utf8(&token[..]) {
        Ok(val) => val,
        Err(_) => return Err(errors::auth::invalid_token()),
    };

    let token = token.split(":").collect::<Vec<&str>>();

    if token.len() != 2 {
        return Err(errors::auth::invalid_token());
    }

    let user_id = decode_id(&token[0]);
    let token = token[1];

    Ok((user_id as i32, token.to_string()))
}

fn get_ip_address(req: &HttpRequest) -> Result<String, ()> {

    if let Some(val) = req.headers().get("CF-Connecting-IP") {
        if let Ok(ip) = val.to_str() {
            return Ok(String::from(ip));
        }
    }

    if let Some(ip) = req.connection_info().peer_addr() {
        return Ok(String::from(ip));
    }

    Err(())
}

pub struct UserInfo {
    user_id: i32,
    access_key: String,
    ip: String,
}

impl UserInfo {
    pub fn user_id(&self) -> i32 { self.user_id }

    pub fn access_key(&self) -> &str { self.access_key.as_ref() }

    pub fn ip(&self) -> &str { self.ip.as_ref() }
}

fn get_user_info(req: &HttpRequest) -> Result<UserInfo, errors::ErrorResponse> {
    let ip = match get_ip_address(&req) {
        Ok(val) => val,
        Err(_) => return Err(errors::ip_unobtainable()),
    };


    let headers = req.headers();
    let (user_id, access_key) = match get_auth_header_info(&headers) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    Ok(UserInfo { user_id, access_key, ip })
}


pub async fn can_access(user_info: &UserInfo, required_type: crate::db_model::UserType) -> Result<bool, errors::ErrorResponse> {
    let mut context = crate::db_model::context().await;

    let user = match User::select_by_id(&mut context, user_info.user_id()).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };

    let user = match user {
        Some(val) => val,
        None => return Err(errors::invalid_user()),
    };


    Ok((user.user_type() as i16) >= (required_type as i16))
}


