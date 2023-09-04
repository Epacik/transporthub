use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, delete, web};
use crate::api::v1::dto;

use crate::db_model::DriverLicense;
use crate::{db_model::{UserType, self}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/drivers-licenses")
    .service(list)
    .service(add)
    .service(remove)
    //.service(update)
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
    let licenses = match DriverLicense::select_all(&mut context).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    let licenses = licenses.iter()
        .map(|x| dto::DriversLicenseDto {
            id: super::encode_id(x.id.unwrap_or_default()),
            driver: super::encode_id(x.driver.clone()),
            license: super::encode_id(x.license.clone()),
        })
        .collect::<Vec<_>>();

    Ok(HttpResponse::Ok().json(licenses))
}

#[put("/add")]
pub async fn add(body: web::Json<dto::DriversLicenseUpdateDto>, req: HttpRequest) -> Response {
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

    let driver_id = super::decode_id(&body.driver);
    let license_id = super::decode_id(&body.license);

    let already_exists = match DriverLicense::select_by_driver_and_license(&mut context, driver_id, license_id).await {
        Ok(val) => if let Some(_) = val { true } else { false },
        Err(err) => return Err(errors::database_error(&err)),
    };

    if already_exists {
        return Err(errors::vehicles::already_exists());
    }

    let _result = match DriverLicense::create(
        &mut context,
        driver_id,
        license_id,
        false).await {
            Ok(val) => val,
            Err(err) => return Err(errors::database_error(&err)),
    };

    let drv_lic = match DriverLicense::select_by_driver_and_license(&mut context, driver_id, license_id).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    match drv_lic {
        None => Err(errors::vehicles::creation_error()),
        Some(drv) => {
            let id = super::encode_id(drv.id.unwrap_or_default());

            let dto = dto::DriversLicenseDto {
                id,
                driver: super::encode_id(drv.driver.clone()),
                license: super::encode_id(drv.license.clone()),
            };

            Ok(HttpResponse::Ok().json(dto))
        },
    }

}

#[delete("/{license_id}/delete")]
pub async fn remove(license_id: web::Path<String>, req: HttpRequest) -> Response {
    let license_id = license_id.clone();
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

    let license_id = super::decode_id(&license_id);

    let mut context = db_model::context().await;
    if let Err(err) = DriverLicense::delete_by_column(&mut context, "id", license_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}
