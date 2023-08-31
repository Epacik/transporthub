use rbatis::{Rbatis, rbdc::Error, impl_select, impl_update, crud};
use serde::{Deserialize, Serialize};
use crate::db_model::table_names;


#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct Vehicle {
    pub id: Option<i32>,
    pub name: String,
    pub vehicle_type: i32,
    pub picture: String,
    pub required_license: i32,
    pub registration_number: String,
    pub vin: String,
    pub disabled: bool,
}

crud!(Vehicle {}, table_names::VEHICLES);
impl_select!(Vehicle { select_by_name(name: &str) -> Option => "`where name = #{name} limit 1`"}, table_names::VEHICLES);
impl_select!(Vehicle { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::VEHICLES);
impl_update!(Vehicle { update_by_id(id: i32) => "`where id = #{id}`"}, table_names::VEHICLES);

impl Vehicle {
    pub async fn count(rb: &Rbatis) -> Result<u64, Error> {
        rb
        .query_decode("select count(id) as count from conf.vehicles", vec![])
        .await
    }

    pub async fn create<S: AsRef<str>>(
        context: &mut dyn rbatis::executor::Executor,
        name: S,
        vehicle_type: i32,
        picture: String,
        required_license: i32,
        registration_number: String,
        vin: String,
        disabled: bool,
    ) -> std::result::Result<rbatis::rbdc::db::ExecResult, rbatis::rbdc::Error> {

        let license = Vehicle {
            id: None,
            name: String::from(name.as_ref()),
            disabled,
            vehicle_type,
            required_license,
            registration_number,
            vin,
            picture,
        };

        match Vehicle::insert( context, &license).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(err),
        }
    }
}
