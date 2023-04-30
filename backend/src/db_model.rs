
use argonautica::{Hasher, Verifier};
use num_derive::FromPrimitive;
use rbatis::{Rbatis, rbdc::{Error, datetime::DateTime}, crud, impl_select, impl_delete};
use rbdc_pg::driver::PgDriver;
use serde::{Deserialize, Serialize};
use rand::distributions::{Alphanumeric, DistString};


use crate::config;

pub async fn context() -> Rbatis {
    log::info!("Creating a database context");
    let config =  config::config();
    let rb = Rbatis::new();

    rb.init(PgDriver {}, &config.connection_string()).unwrap();
    rb.try_acquire().await.unwrap();

    rb
}

#[derive(Copy, Clone, FromPrimitive, Serialize, Deserialize)]
pub enum UserType {
    Invalid = -1,
    User = 1,
    Manager = 2,
    Admin = 3,
}

pub enum PasswordVerificationResult {
    Authorized,
    InvalidPassword,
    Expired,
}

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct User {
    pub id: Option<i32>,
    pub name: String,
    pub picture: Option<String>,
    pub password_salt: String,
    pub password_hash: String,
    pub password_expiration_date: Option<DateTime>,
    user_type: i16,
    pub multi_login: bool,
    pub disabled: bool
}

crud!(User {}, table_names::USERS);
impl_select!(User { select_by_name(name: &str) -> Option => "`where name = #{name} limit 1`"}, table_names::USERS);
impl_select!(User { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::USERS);



pub enum UserCreateError {
    HashError(argonautica::Error),
    DatabaseError(rbatis::rbdc::Error),
}


impl User {
    pub async fn count(rb: &Rbatis) -> Result<u64, Error> {
        rb
        .query_decode("select count(id) as count from conf.users", vec![])
        .await
    }
    pub fn make_salt() -> String {
        Alphanumeric.sample_string(&mut rand::thread_rng(), 64)
    }


    pub async fn create<S: AsRef<str>>(
        context: &mut dyn rbatis::executor::Executor,
        name: S,
        picture: Option<String>,
        password: S,
        password_expiration_date: Option<fastdate::DateTime>,
        usertype: UserType,
        multi_login: bool,
        disabled: bool
    ) -> std::result::Result<rbatis::rbdc::db::ExecResult, UserCreateError> {
        let config = config::config();

        let salt = User::make_salt();
        let mut hasher = Hasher::default();

        let hash = hasher
            .with_password(password.as_ref())
            .with_secret_key(config.pass_secret())
            .with_salt(&salt)
            .hash();

        let hash = match hash {
            Ok(val) => val,
            Err(err) => return Err(UserCreateError::HashError(err)),
        };

        let exp_date = match password_expiration_date {
            Some(d) => Some(DateTime(d)),
            None => None,
        };

        let admin = User {
            id: None,
            name: String::from(name.as_ref()),
            picture: picture,
            password_salt: salt,
            password_hash: hash,
            password_expiration_date: exp_date,
            user_type: usertype as i16,
            multi_login,
            disabled,
        };

        match User::insert( context, &admin).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(UserCreateError::DatabaseError(err)),
        }
    }

    pub async fn check_password<S: AsRef<str>>(&self, password: S) -> PasswordVerificationResult {
        let now = fastdate::DateTime::utc();

        if let Some(d) = &self.password_expiration_date {
            if now > d.0 {
                return PasswordVerificationResult::Expired;
            }
        }


        let config = config::config();
        let mut verifier = Verifier::default();
        let ver = verifier
            .with_password(password.as_ref())
            .with_secret_key(config.pass_secret())
            .with_hash(&self.password_hash)
            .verify()
            .unwrap();

        if ver {
            PasswordVerificationResult::Authorized
        }
        else {
            PasswordVerificationResult::InvalidPassword
        }
    }

    pub fn user_type(&self) -> UserType {
        num::FromPrimitive::from_i16(self.user_type).unwrap_or(UserType::Invalid)
    }
    pub fn set_user_type(&mut self, user_type: UserType) {
        self.user_type = user_type as i16;
    }
}

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct UserAccessKeys {
    pub id: Option<i32>,
    pub user_id: i32,
    pub key: String,
    pub last_refreshed: DateTime,
    pub device_id: String,
}

crud!(UserAccessKeys {}, table_names::USER_ACCESS_KEYS);
impl_select!(UserAccessKeys { select_all_by_user_and_device(user: i32, device: &str) => "`where user = #{user} and device_id: #{device}`" }, table_names::USER_ACCESS_KEYS);
impl_select!(UserAccessKeys { select_all_by_user(user: i32) => "`where user_id = #{user}`"}, table_names::USER_ACCESS_KEYS);
impl_select!(UserAccessKeys { select_older_than(datetime: rbatis::rbdc::datetime::DateTime) => "`where last_refreshed < #{datetime}`"}, table_names::USER_ACCESS_KEYS);
impl_select!(UserAccessKeys { select_by_key(key: &str) -> Option => "`where key = #{key}`"}, table_names::USER_ACCESS_KEYS);
impl_delete!(UserAccessKeys { delete(id: i32) => "`where id = #{id}`" }, table_names::USER_ACCESS_KEYS);

impl UserAccessKeys {
    pub(crate) async fn create<S: AsRef<str>>(context: &mut dyn rbatis::executor::Executor, key: S, user_id: i32, device_id: &String) -> Result<(), rbatis::rbdc::Error> {
        let now = DateTime::utc();

        UserAccessKeys::insert(context, &UserAccessKeys {
            id: None,
            user_id,
            key: String::from(key.as_ref()),
            last_refreshed: now,
            device_id: device_id.clone(),
        }).await?;

        Ok(())
    }
}


mod table_names {
    pub const USERS: &str = "conf.users";
    pub const USER_ACCESS_KEYS: &str = "conf.user_access_keys";
}
