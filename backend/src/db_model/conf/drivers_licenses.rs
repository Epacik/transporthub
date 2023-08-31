use rbatis::{Rbatis, rbdc::Error, crud, impl_select, impl_update};
use serde::{Deserialize, Serialize};

use crate::db_model::table_names;


#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct DriverLicense {
    pub id: Option<i32>,
    pub driver: i32,
    pub license: i32,
    pub disabled: bool,
}

crud!(DriverLicense {}, table_names::DRIVERS_LICENSES);
impl_select!(DriverLicense { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::DRIVERS_LICENSES);
impl_select!(DriverLicense { select_by_driver_and_license(driver: i32, license: i32) -> Option => "`where driver = #{driver} and license = #{license} limit 1`"}, table_names::DRIVERS_LICENSES);
impl_update!(DriverLicense { update_by_id(id: i32) => "`where id = #{id}`"}, table_names::DRIVERS_LICENSES);

impl DriverLicense {
    pub async fn count(rb: &Rbatis) -> Result<u64, Error> {
        rb
        .query_decode("select count(id) as count from conf.drivers_licenses", vec![])
        .await
    }

    pub async fn create(
        context: &mut dyn rbatis::executor::Executor,
        driver: i32,
        license: i32,
        disabled: bool,
    ) -> std::result::Result<rbatis::rbdc::db::ExecResult, rbatis::rbdc::Error> {

        let license = DriverLicense {
            id: None,
            driver,
            license,
            disabled,
        };

        match DriverLicense::insert( context, &license).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(err),
        }
    }
}
