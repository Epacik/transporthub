use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get, put, delete, patch, web};
use crate::api::v1::dto;

use crate::db_model::Vehicle;
use crate::{db_model::{UserType, self}, errors};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/vehicles")
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
    let licenses = match Vehicle::select_all(&mut context).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    let licenses = licenses.iter()
        .map(|x| dto::VehicleDto {
            id: super::encode_id(x.id.unwrap_or_default()),
            name: x.name.clone(),
            vehicle_type: x.vehicle_type.clone(),
            picture: x.picture.clone(),
            required_license: super::encode_id(x.required_license.clone()),
            registration_number: x.registration_number.clone(),
            vin: x.vin.clone(),
            disabled: x.disabled != 0,
        })
        .collect::<Vec<_>>();

    Ok(HttpResponse::Ok().json(licenses))
}


#[put("/add")]
pub async fn add(body: web::Json<dto::VehicleUpdateDto>, req: HttpRequest) -> Response {
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

    let already_exists = match Vehicle::select_by_name(&mut context, &body.name).await {
        Ok(val) => if let Some(_) = val { true } else { false },
        Err(err) => return Err(errors::database_error(&err)),
    };

    if already_exists {
        return Err(errors::vehicles::already_exists());
    }

    let _result = match Vehicle::create(
        &mut context,
        body.name.clone(),
        body.vehicle_type.clone(),
        body.picture.clone(),
        super::decode_id(&body.required_license.clone()),
        body.registration_number.clone(),
        body.vin.clone(),
        body.disabled.clone()).await {
            Ok(val) => val,
            Err(err) => return Err(errors::database_error(&err)),
    };

    let name = body.name.clone();

    let vehicle = match Vehicle::select_by_name(&mut context, name.as_str()).await {
        Ok(val) => val,
        Err(err) => return Err(errors::database_error(&err)),
    };


    match vehicle {
        None => Err(errors::vehicles::creation_error()),
        Some(lic) => {
            let id = super::encode_id(lic.id.unwrap_or_default());

            let dto = dto::VehicleDto {
                id,
                name: lic.name.clone(),
                vehicle_type: lic.vehicle_type.clone(),
                picture: lic.picture.clone(),
                required_license: super::encode_id(lic.required_license.clone()),
                registration_number: lic.registration_number.clone(),
                vin: lic.vin.clone(),
                disabled: lic.disabled != 0,
            };

            Ok(HttpResponse::Ok().json(dto))
        },
    }

}

#[delete("/{vehicle_id}/delete")]
pub async fn remove(vehicle_id: web::Path<String>, req: HttpRequest) -> Response {
    let vehicle_id = vehicle_id.clone();
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

    let vehicle_id = super::decode_id(&vehicle_id);

    let mut context = db_model::context().await;
    if let Err(err) = Vehicle::delete_by_column(&mut context, "id", vehicle_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

#[patch("/{vehicle_id}/update")]
pub async fn update(vehicle_id: web::Path<String>, body: web::Json<dto::VehicleUpdateDto>, req: HttpRequest) -> Response {
    let vehicle_id = vehicle_id.clone();
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let can_access = match super::can_access(&user_info, UserType::Manager).await {
        Ok(val) => val,
        Err(err) => return Err(err),
    };

    let vehicle_id = super::decode_id(&vehicle_id);

    if !can_access {
        return Err(errors::insufficient_privileges(user_info.user_id));
    }

    let mut context = db_model::context().await;
    let vehicle = match Vehicle::select_by_id(&mut context, vehicle_id).await {
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
    vehicle.vehicle_type = dto.vehicle_type.clone();
    vehicle.picture = dto.picture.clone();
    vehicle.required_license = super::decode_id(&dto.required_license.clone());
    vehicle.registration_number = dto.registration_number.clone();
    vehicle.vin = dto.vin.clone();
    vehicle.disabled = if dto.disabled { 1 } else { 0 };

    if let Err(err) = Vehicle::update_by_id(&mut context, &vehicle, vehicle_id).await {
        return Err(errors::database_error(&err));
    }

    Ok(HttpResponse::Ok().finish())
}

