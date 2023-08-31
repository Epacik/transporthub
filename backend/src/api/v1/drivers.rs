use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, delete, patch, web};
use crate::api::v1::dto;

use crate::db_model::Driver;
use crate::{db_model::{UserType, self}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/drivers")
    .service(list)
    .service(add)
    .service(remove)
    .service(update)
}


#[get("/list")]
pub async fn list(req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::User).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id()));
    }

    let mut context = db_model::context().await;
    let licenses = match Driver::select_all(&mut context).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    let licenses = licenses.iter()
        .map(|x| dto::DriverDto {
            id: super::encode_id(x.id.unwrap_or_default()),
            name: x.name.clone(),
            picture: match &x.picture {
                None => None,
                Some(p) => Some(p.clone()),
            },
            nationality: x.nationality.clone(),
            base_location: x.base_location.clone(),
            disabled: x.disabled,
        })
        .collect::<Vec<_>>();

    Ok(HttpResponse::Ok().json(licenses))
}


#[put("/add")]
pub async fn add(body: web::Json<dto::DriverUpdateDto>, req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Manager).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id()));
    }

    let mut context = db_model::context().await;

    let already_exists = match Driver::select_by_name(&mut context, &body.name).await {
        Ok(val) => if let Some(_) = val { true } else { false },
        Err(err) => return Err(errors::database_error(&err)),
    };

    if already_exists {
        return Err(errors::vehicles::already_exists());
    }

    let _result = match Driver::create(
        &mut context,
        body.name.clone(),
        body.picture.clone(),
        body.nationality.clone(),
        body.base_location.clone(),
        body.disabled.clone()).await {
            Ok(val) => val,
            Err(err) => return Err(errors::database_error(&err)),
    };

    let name = body.name.clone();

    let vehicle = match Driver::select_by_name(&mut context, name.as_str()).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    match vehicle {
        None => Err(errors::vehicles::creation_error()),
        Some(drv) => {
            let id = super::encode_id(drv.id.unwrap_or_default());

            let dto = dto::DriverDto {
                id,
                name: drv.name.clone(),
                picture: match &drv.picture {
                    None => None,
                    Some(p) => Some(p.clone()),
                },
                nationality: drv.nationality.clone(),
                base_location: drv.base_location.clone(),
                disabled: drv.disabled,
            };

            Ok(HttpResponse::Ok().json(dto))
        },
    }

}

#[delete("/{driver_id}/delete")]
pub async fn remove(driver_id: web::Path<String>, req: HttpRequest) -> Response {
    let driver_id = driver_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Manager).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id()));
    }

    let driver_id = super::decode_id(&driver_id);

    let mut context = db_model::context().await;
    if let Err(err) = Driver::delete_by_column(&mut context, "id", driver_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{driver_id}/update")]
pub async fn update(driver_id: web::Path<String>, body: web::Json<dto::DriverUpdateDto>, req: HttpRequest) -> Response {
    let driver_id = driver_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Manager).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let driver_id = super::decode_id(&driver_id);

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let vehicle = match Driver::select_by_id(&mut context, driver_id).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };

    let mut vehicle = match vehicle {
        Some(val) => val,
        None => return Err(errors::not_found()),
    };

    let dto = body.0;
    vehicle.id = None;
    vehicle.name = dto.name.clone();
    vehicle.picture = dto.picture.clone();
    vehicle.nationality = dto.nationality.clone();
    vehicle.base_location = dto.base_location.clone();
    vehicle.disabled = dto.disabled.clone();

    if let Err(err) = Driver::update_by_id(&mut context, &vehicle, driver_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

