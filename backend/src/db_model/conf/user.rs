use argonautica::{ Hasher, Verifier };
use num_derive::FromPrimitive;
use rbatis::{
    RBatis,
    rbdc::{ Error, datetime::DateTime },
    crud,
    impl_select,
    impl_delete,
    impl_update,
};
use serde::{ Deserialize, Serialize };
use rand::distributions::{ Alphanumeric, DistString };

use crate::{ db_model::table_names, config };

#[derive(Copy, Clone, FromPrimitive, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
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
    pub multi_login: i32,
    pub disabled: i32,
}

crud!(User {}, table_names::USERS);
impl_select!(User { select_by_name(name: &str) -> Option => "`where name = #{name} limit 1`"}, table_names::USERS);
impl_select!(User { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::USERS);
impl_update!(User { update_by_id(id: i32) => "`where id = #{id}`"}, table_names::USERS);

pub enum UserCreateError {
    HashError(argonautica::Error),
    DatabaseError(rbatis::rbdc::Error),
}

impl User {
    pub async fn count(rb: &RBatis) -> Result<u64, Error> {
        rb.query_decode("select count(id) as count from conf.users", vec![]).await
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
            Err(err) => {
                return Err(UserCreateError::HashError(err));
            }
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
            multi_login: if multi_login { 1 } else { 0 },
            disabled: if disabled { 1 } else { 0 },
        };

        match User::insert(context, &admin).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(UserCreateError::DatabaseError(err)),
        }
    }

    pub fn set_password<S: AsRef<str>>(
        &mut self,
        password: S
    ) -> Result<String, argonautica::Error> {
        let config = config::config();

        let salt = self.password_salt.clone();
        let mut hasher = Hasher::default();

        let hash = hasher
            .with_password(password.as_ref())
            .with_secret_key(config.pass_secret())
            .with_salt(&salt)
            .hash();

        if hash.is_ok() {
            self.password_hash = hash.clone().unwrap();
        }

        hash
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
        } else {
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

impl Default for User {
    fn default() -> Self {
        Self {
            id: Some(0),
            name: Default::default(),
            picture: Some(String::new()),
            password_salt: Default::default(),
            password_hash: Default::default(),
            password_expiration_date: Some(DateTime::now()),
            user_type: UserType::User as i16,
            multi_login: Default::default(),
            disabled: Default::default(),
        }
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
    pub(crate) async fn create<S: AsRef<str>>(
        context: &mut dyn rbatis::executor::Executor,
        key: S,
        user_id: i32,
        device_id: &String
    ) -> Result<(), rbatis::rbdc::Error> {
        let now = DateTime::utc();

        UserAccessKeys::insert(
            context,
            &(UserAccessKeys {
                id: None,
                user_id,
                key: String::from(key.as_ref()),
                last_refreshed: now,
                device_id: device_id.clone(),
            })
        ).await?;

        Ok(())
    }
}

impl Default for UserAccessKeys {
    fn default() -> Self {
        Self {
            id: Some(1),
            user_id: 1,
            key: String::new(),
            last_refreshed: DateTime::now(),
            device_id: String::new(),
        }
    }
}
