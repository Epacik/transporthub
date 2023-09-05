use std::sync::Arc;

use rbatis::{Error, RBatis};
use rbatis::executor::Executor;
use rbatis::intercept::{Intercept, ResultType};
use rbatis::rbdc::db::ExecResult;
use rbs::Value;
use rbatis::table_sync::SqliteTableSync;
use rbatis::table_sync::TableSync;
use rbdc_pg::driver::PgDriver;
use rbdc_sqlite::driver::SqliteDriver;
use rbs::to_value;
use crate::config;
use crate::config::DatabaseType;


mod conf;
mod orders;

pub use self::conf::user::*;
pub use self::conf::licenses::*;
pub use self::conf::vehicles::*;
pub use self::conf::drivers::*;
pub use self::conf::drivers_licenses::*;


pub async fn context() -> RBatis {
    log::info!("Creating a database context");
    let config =  config::config();
    let rb = RBatis::new();

    match config.used_database() {
        DatabaseType::PostgreSQL => rb.init(PgDriver {}, &config.connection_string()),
        DatabaseType::Sqlite => {
            rb.intercepts.push(Arc::new(SqliteTableNameInterceptor{}) as Arc<dyn Intercept>);
            rb.init(SqliteDriver {}, &config.connection_string())
        },
    }.unwrap();

    rb.try_acquire().await.unwrap();

    rb
}

pub async fn sync_tables() {
    let config =  config::config();

    if let DatabaseType::PostgreSQL = config.used_database() {
        return;
    }

    sync_sqlite_tables().await;
}

async fn sync_sqlite_tables() {
    let context = context().await;

    let mut sync = SqliteTableSync::default();
    sync.sql_id = " PRIMARY KEY AUTOINCREMENT NOT NULL ".to_string();

    sync.sync(context.try_acquire().await.unwrap(), to_value!(User::default()), table_names::USERS).await.unwrap();
    sync.sync(context.try_acquire().await.unwrap(), to_value!(UserAccessKeys::default()), table_names::USER_ACCESS_KEYS).await.unwrap();
    sync.sync(context.try_acquire().await.unwrap(), to_value!(LicenseType::default()), table_names::LICENSES).await.unwrap();
    sync.sync(context.try_acquire().await.unwrap(), to_value!(Vehicle::default()), table_names::VEHICLES).await.unwrap();
    sync.sync(context.try_acquire().await.unwrap(), to_value!(Driver::default()), table_names::DRIVERS).await.unwrap();
    sync.sync(context.try_acquire().await.unwrap(), to_value!(DriverLicense::default()), table_names::DRIVERS_LICENSES).await.unwrap();
}


mod table_names {
    pub const USERS: &str = "conf.users";
    pub const USER_ACCESS_KEYS: &str = "conf.user_access_keys";
    pub const LICENSES: &str = "conf.license_types";
    pub const VEHICLES: &str = "conf.vehicles";
    pub const DRIVERS: &str = "conf.drivers";
    pub const DRIVERS_LICENSES: &str = "conf.drivers_licenses";
}

#[derive(Debug)]
pub struct SqliteTableNameInterceptor{}

impl Intercept for SqliteTableNameInterceptor {
    /// task_id maybe is conn_id or tx_id,
    /// is_prepared_sql = !args.is_empty(),
    /// if return Ok(false) will be return data. return Ok(true) will run next
    fn before(
        &self,
        _task_id: i64,
        _rb: &dyn Executor,
        _sql: &mut String,
        _args: &mut Vec<Value>,
        _result: ResultType<&mut Result<ExecResult, Error>, &mut Result<Vec<Value>, Error>>,
    ) -> Result<bool, Error> {

        *_sql = _sql.replace("conf.", "conf_").replace("orders.", "orders_");

        Ok(true)
    }

    /// task_id maybe is conn_id or tx_id,
    /// is_prepared_sql = !args.is_empty(),
    /// if return Ok(false) will be return data. return Ok(true) will run next
    fn after(
        &self,
        _task_id: i64,
        _rb: &dyn Executor,
        _sql: &mut String,
        _args: &mut Vec<Value>,
        _result: ResultType<&mut Result<ExecResult, Error>, &mut Result<Vec<Value>, Error>>,
    ) -> Result<bool, Error> {
        Ok(true)
    }
}
