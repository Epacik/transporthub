use fastdate::DateTime;
use serde::{Deserialize, Serialize};

use crate::db_model::UserType;

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LoginRequestDto {
    pub user: String,
    pub password: String,
    pub disconnect_other_sessions: Option<bool>,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct LoginResponseDto {
    pub user: String,
    pub key: String,
}


impl LoginResponseDto {
    pub fn new(user: String, key: String) -> LoginResponseDto {
        LoginResponseDto {
            user,
            key,
        }
    }
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

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserAdminUpdateDto {
    pub name: String,
    pub picture: Option<String>,
    pub password_expiration_date: Option<DateTime>,
    pub user_type: UserType,
    pub multi_login: bool,
    pub disabled: bool,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserUpdateDto {
    pub name: String,
    pub picture: Option<String>,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct UserUpdatePasswordDto {
    pub password: String,
}


#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct LicenseTypeDto {
    pub id: String,
    pub name: String,
    pub description: String,
    pub minimal_age_of_holder: i32,
    pub alternative_minimal_age_of_holder: Option<i32>,
    pub condition_for_alternative_minimal_age: Option<String>,
    pub disabled: bool,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct LicenseTypeUpdateDto {
    pub name: String,
    pub description: String,
    pub minimal_age_of_holder: i32,
    pub alternative_minimal_age_of_holder: Option<i32>,
    pub condition_for_alternative_minimal_age: Option<String>,
    pub disabled: bool,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct VehicleDto {
    pub id: String,
    pub name: String,
    pub vehicle_type: i32,
    pub picture: String,
    pub required_license: i32,
    pub registration_number: String,
    pub vin: String,
    pub disabled: bool,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct VehicleUpdateDto {
    pub name: String,
    pub vehicle_type: i32,
    pub picture: String,
    pub required_license: i32,
    pub registration_number: String,
    pub vin: String,
    pub disabled: bool,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct DriverDto {
    pub id: String,
    pub name: String,
    pub picture: Option<String>,
    pub nationality: String,
    pub base_location: String,
    pub disabled: bool,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct DriverUpdateDto {
    pub name: String,
    pub picture: Option<String>,
    pub nationality: String,
    pub base_location: String,
    pub disabled: bool,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct DriversLicenseDto {
    pub id: String,
    pub driver: i32,
    pub license: i32,
    pub disabled: bool,
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct DriversLicenseUpdateDto {
    pub driver: i32,
    pub license: i32,
    pub disabled: bool,
}
