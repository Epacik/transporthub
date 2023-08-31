use rbatis::{Rbatis, rbdc::Error, crud, impl_select, impl_update};
use serde::{Deserialize, Serialize};

use crate::db_model::table_names;


#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct Driver {
    pub id: Option<i32>,
    pub name: String,
    pub picture: Option<String>,
    pub nationality: String,
    pub base_location: String,
    pub disabled: bool,
}

crud!(Driver {}, table_names::DRIVERS);
impl_select!(Driver { select_by_name(name: &str) -> Option => "`where name = #{name} limit 1`"}, table_names::DRIVERS);
impl_select!(Driver { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::DRIVERS);
impl_update!(Driver { update_by_id(id: i32) => "`where id = #{id}`"}, table_names::DRIVERS);

impl Driver {
    pub async fn count(rb: &Rbatis) -> Result<u64, Error> {
        rb
        .query_decode("select count(id) as count from conf.drivers", vec![])
        .await
    }

    pub async fn create<S: AsRef<str>>(
        context: &mut dyn rbatis::executor::Executor,
        name: S,
        picture: Option<String>,
        nationality: String,
        base_location: String,
        disabled: bool,
    ) -> std::result::Result<rbatis::rbdc::db::ExecResult, rbatis::rbdc::Error> {

        let license = Driver {
            id: None,
            name: String::from(name.as_ref()),
            picture,
            nationality,
            base_location,
            disabled,
        };

        match Driver::insert( context, &license).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(err),
        }
    }
}
