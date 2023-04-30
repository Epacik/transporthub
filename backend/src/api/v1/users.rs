use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, web};
use fastdate::DateTime;
use serde::{Deserialize, Serialize};

use crate::{db_model::{UserType, User, self, UserCreateError}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/users")
    .service(list)
    .service(add)
}


#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct UserDto {
    pub id: String,
    pub name: String,
    pub picture: Option<String>,
    pub password_expiration_date: Option<DateTime>,
    pub user_type: UserType,
    pub multi_login: bool,
    pub disabled: bool,
}

#[get("/list")]
pub async fn list(req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id()));
    }

    let mut context = db_model::context().await;
    let users = match User::select_all(&mut context).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };

    let hash_ids = super::get_hashid();

    let users = users.iter()
        .map(|x| UserDto {
            id: hash_ids.encode(&[x.id.unwrap_or_default() as u64]),
            name: x.name.clone(),
            picture: x.picture.clone(),
            password_expiration_date: match x.password_expiration_date.clone() {
                Some(val) => Some(val.0),
                None => None,
            },
            user_type: x.user_type(),
            multi_login: x.multi_login,
            disabled: x.disabled,
        })
        .collect::<Vec<_>>();

    Ok(HttpResponse::Ok().json(users))
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserAddDto {
    pub name: String,
    pub picture: Option<String>,
    pub password: String,
    pub password_expiration_date: Option<DateTime>,
    pub user_type: UserType,
    pub multi_login: bool,
}

#[put("/add")]
pub async fn add(body: web::Json<UserAddDto>, req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id()));
    }

    let mut context = db_model::context().await;

    let already_exists = match User::select_by_name(&mut context, &body.name).await {
        Ok(val) => if let Some(_) = val { true } else { false },
        Err(err) => return Err(errors::database_error(&err)),
    };

    if already_exists {
        return Err(errors::users::already_exists());
    }

    let result = match User::create(
        &mut context,
        body.name.clone(),
        body.picture.clone(),
        body.password.clone(),
        body.password_expiration_date.clone(),
        body.user_type,
        body.multi_login,
        true).await {
            Ok(val) => val,
            Err(err) => return Err(match err {
                UserCreateError::HashError(hr) => errors::hashing_error(&hr),
                UserCreateError::DatabaseError(dr) => errors::database_error(&dr),
            }),
    };

    let name = body.name.clone();

    let user = match User::select_by_name(&mut context, name.as_str()).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };

    let hash_ids = super::get_hashid();

    match user {
        None => Err(errors::users::creation_error()),
        Some(user) => {
            let id = hash_ids.encode(&[user.id.unwrap_or_default() as u64]);

            let dto = UserDto {
                id,
                name: user.name.clone(),
                picture: user.picture.clone(),
                password_expiration_date: match user.password_expiration_date.clone() {
                    Some(val) => Some(val.0),
                    None => None,
                },
                user_type: user.user_type().clone(),
                multi_login: user.multi_login,
                disabled: user.disabled
            };

            Ok(HttpResponse::Ok().json(dto))
        },
    }

}
