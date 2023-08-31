use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, delete, patch, web};
use crate::api::v1::dto;

use crate::{db_model::{UserType, User, self, UserCreateError}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/users")
    .service(list)
    .service(get_user)
    .service(add)
    .service(remove)
    .service(update)
    .service(update_as_admin)
    .service(update_password)
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


    let users = users.iter()
        .map(|x| dto::UserDto {
            id: super::encode_id(x.id.unwrap_or_default()),
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


#[get("/{user_id}")]
pub async fn get_user(user_id: web::Path<String>, req: HttpRequest) -> Response {
    let user_id = user_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };


    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let user_id = super::decode_id(&user_id);

    if user_info.user_id != user_id && !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let user = match User::select_by_id(&mut context, user_id).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };

    let user = match user {
        None => return Err(errors::not_found()),
        Some(x) => dto::UserDto {
            id: super::encode_id(x.id.unwrap_or_default()),
            name: x.name.clone(),
            picture: x.picture.clone(),
            password_expiration_date: match x.password_expiration_date.clone() {
                Some(val) => Some(val.0),
                None => None,
            },
            user_type: x.user_type(),
            multi_login: x.multi_login,
            disabled: x.disabled,
        },
    };

    Ok(HttpResponse::Ok().json(user))

}

#[put("admin/add")]
pub async fn add(body: web::Json<dto::UserAddDto>, req: HttpRequest) -> Response {
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

    let _result = match User::create(
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


    match user {
        None => Err(errors::users::creation_error()),
        Some(user) => {
            let id = super::encode_id(user.id.unwrap_or_default());

            let dto = dto::UserDto {
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

#[delete("/{user_id}/admin/delete")]
pub async fn remove(user_id: web::Path<String>, req: HttpRequest) -> Response {
    let user_id = user_id.clone();
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

    let user_id = super::decode_id(&user_id);

    let mut context = db_model::context().await;
    if let Err(err) = User::delete_by_column(&mut context, "id", user_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{user_id}/update")]
pub async fn update(user_id: web::Path<String>, body: web::Json<dto::UserUpdateDto>, req: HttpRequest) -> Response {
    let user_id = user_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let user_id = super::decode_id(&user_id);

    if user_info.user_id != user_id && !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let user = match User::select_by_id(&mut context, user_id).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };

    let mut user = match user {
        Some(val) => val,
        None => return Err(errors::not_found()),
    };

    user.name = body.0.name.clone();
    user.picture = body.0.picture.clone();
    user.id = None;

    if let Err(err) = User::update_by_id(&mut context, &user, user_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{user_id}/admin/update")]
pub async fn update_as_admin(user_id: web::Path<String>, body: web::Json<dto::UserAdminUpdateDto>, req: HttpRequest) -> Response {
    let user_id = user_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let user_id = super::decode_id(&user_id);

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let user = match User::select_by_id(&mut context, user_id).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };

    let mut user = match user {
        Some(val) => val,
        None => return Err(errors::not_found()),
    };

    let dto = body.0;
    user.id = None;
    user.name = dto.name.clone();
    user.picture = dto.picture.clone();
    user.password_expiration_date = match dto.password_expiration_date.clone() {
        None => None,
        Some(d) => Some(rbatis::rbdc::datetime::DateTime(d)),
    };
    user.set_user_type(dto.user_type.clone());
    user.multi_login = dto.multi_login;
    user.disabled = dto.disabled;


    if let Err(err) = User::update_by_id(&mut context, &user, user_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{user_id}/update_password")]
pub async fn update_password(user_id: web::Path<String>, body: web::Json<dto::UserUpdatePasswordDto>, req: HttpRequest) -> Response {
    let user_id = user_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Admin).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let user_id = super::decode_id(&user_id);

    if user_info.user_id != user_id && !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let user = match User::select_by_id(&mut context, user_id).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };

    let mut user = match user {
        Some(val) => val,
        None => return Err(errors::not_found()),
    };

    if let Err(e) = user.set_password(body.password.clone()) {
        return Err(errors::password_change_error(&e));
    }

    user.id = None;

    if let Err(err) = User::update_by_id(&mut context, &user, user_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}
