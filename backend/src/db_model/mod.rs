use rbatis::Rbatis;
use rbdc_pg::driver::PgDriver;
use crate::config;


mod conf;
mod orders;

pub use self::conf::user::*;
pub use self::conf::licenses::*;
pub use self::conf::vehicles::*;
pub use self::conf::drivers::*;
pub use self::conf::drivers_licenses::*;


pub async fn context() -> Rbatis {
    log::info!("Creating a database context");
    let config =  config::config();
    let rb = Rbatis::new();

    rb.init(PgDriver {}, &config.connection_string()).unwrap();
    rb.try_acquire().await.unwrap();

    rb
}


mod table_names {
    pub const USERS: &str = "conf.users";
    pub const USER_ACCESS_KEYS: &str = "conf.user_access_keys";
    pub const LICENSES: &str = "conf.license_types";
    pub const VEHICLES: &str = "conf.vehicles";
    pub const DRIVERS: &str = "conf.drivers";
    pub const DRIVERS_LICENSES: &str = "conf.drivers_licenses";
}
