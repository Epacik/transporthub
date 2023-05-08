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
pub struct UserUpdateDto {
    pub name: String,
    pub picture: Option<String>,
}
