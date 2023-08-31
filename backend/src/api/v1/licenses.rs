use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, delete, patch, web};
use crate::api::v1::dto;

use crate::db_model::LicenseType;
use crate::{db_model::{UserType, self}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/licenses")
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
    let licenses = match LicenseType::select_all(&mut context).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    let licenses = licenses.iter()
        .map(|x| dto::LicenseTypeDto {
            id: super::encode_id(x.id.unwrap_or_default()),
            name: x.name.clone(),
            description: x.description.clone(),
            minimal_age_of_holder: x.minimal_age_of_holder.clone(),
            alternative_minimal_age_of_holder: x.alternative_minimal_age_of_holder.clone(),
            condition_for_alternative_minimal_age: x.condition_for_alternative_minimal_age.clone(),
            disabled: x.disabled,
        })
        .collect::<Vec<_>>();

    Ok(HttpResponse::Ok().json(licenses))
}


#[put("/add")]
pub async fn add(body: web::Json<dto::LicenseTypeUpdateDto>, req: HttpRequest) -> Response {
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

    let already_exists = match LicenseType::select_by_name(&mut context, &body.name).await {
        Ok(val) => if let Some(_) = val { true } else { false },
        Err(err) => return Err(errors::database_error(&err)),
    };

    if already_exists {
        return Err(errors::licenses::already_exists());
    }

    let _result = match LicenseType::create(
        &mut context,
        body.name.clone(),
        body.description.clone(),
        body.minimal_age_of_holder.clone(),
        body.alternative_minimal_age_of_holder.clone(),
        body.condition_for_alternative_minimal_age.clone(),
        false).await {
            Ok(val) => val,
            Err(err) => return Err(errors::database_error(&err)),
    };

    let name = body.name.clone();

    let license = match LicenseType::select_by_name(&mut context, name.as_str()).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    match license {
        None => Err(errors::licenses::creation_error()),
        Some(lic) => {
            let id = super::encode_id(lic.id.unwrap_or_default());

            let dto = dto::LicenseTypeDto {
                id,
                name: lic.name.clone(),
                description: lic.description.clone(),
                minimal_age_of_holder: lic.minimal_age_of_holder.clone(),
                alternative_minimal_age_of_holder: lic.alternative_minimal_age_of_holder.clone(),
                condition_for_alternative_minimal_age: lic.condition_for_alternative_minimal_age.clone(),
                disabled: lic.disabled,
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
    if let Err(err) = LicenseType::delete_by_column(&mut context, "id", license_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{license_id}/update")]
pub async fn update(license_id: web::Path<String>, body: web::Json<dto::LicenseTypeUpdateDto>, req: HttpRequest) -> Response {
    let license_id = license_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Manager).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let license_id = super::decode_id(&license_id);

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let license = match LicenseType::select_by_id(&mut context, license_id).await {
        Err(err) => return Err(errors::database_error(&err)),
        Ok(val) => val,
    };

    let mut license = match license {
        Some(val) => val,
        None => return Err(errors::not_found()),
    };

    let dto = body.0;
    license.id = None;
    license.name = dto.name.clone();
    license.minimal_age_of_holder = dto.minimal_age_of_holder.clone();
    license.alternative_minimal_age_of_holder = dto.alternative_minimal_age_of_holder.clone();
    license.condition_for_alternative_minimal_age = dto.condition_for_alternative_minimal_age.clone();
    license.disabled = dto.disabled;

    if let Err(err) = LicenseType::update_by_id(&mut context, &license, license_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

