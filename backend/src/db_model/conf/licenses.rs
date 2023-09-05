use rbatis::{ RBatis, rbdc::Error, impl_select, impl_update, crud };
use serde::{ Deserialize, Serialize };
use crate::db_model::table_names;

#[derive(Clone, Debug, Serialize, Deserialize)]
pub struct LicenseType {
    pub id: Option<i32>,
    pub name: String,
    pub description: String,
    pub minimal_age_of_holder: i32,
    pub alternative_minimal_age_of_holder: Option<i32>,
    pub condition_for_alternative_minimal_age: Option<String>,
    pub disabled: i32,
}

crud!(LicenseType {}, table_names::LICENSES);
impl_select!(LicenseType { select_by_name(name: &str) -> Option => "`where name = #{name} limit 1`"}, table_names::LICENSES);
impl_select!(LicenseType { select_by_id(id: i32) -> Option => "`where id = #{id} limit 1`"}, table_names::LICENSES);
impl_update!(LicenseType { update_by_id(id: i32) => "`where id = #{id}`"}, table_names::LICENSES);

impl LicenseType {
    pub async fn count(rb: &RBatis) -> Result<u64, Error> {
        rb.query_decode("select count(id) as count from conf.license_types", vec![]).await
    }

    pub async fn create<S: AsRef<str>>(
        context: &mut dyn rbatis::executor::Executor,
        name: S,
        description: String,
        minimal_age_of_holder: i32,
        alternative_minimal_age_of_holder: Option<i32>,
        condition_for_alternative_minimal_age: Option<String>,
        disabled: bool
    ) -> std::result::Result<rbatis::rbdc::db::ExecResult, rbatis::rbdc::Error> {
        let disabled = if disabled { 1 } else { 0 };
        let license = LicenseType {
            id: None,
            name: String::from(name.as_ref()),
            disabled,
            description,
            minimal_age_of_holder,
            alternative_minimal_age_of_holder,
            condition_for_alternative_minimal_age,
        };

        match LicenseType::insert(context, &license).await {
            Ok(ok) => Ok(ok),
            Err(err) => Err(err),
        }
    }
}

impl Default for LicenseType {
    fn default() -> Self {
        Self {
            id: Some(1),
            name: String::new(),
            description: String::new(),
            minimal_age_of_holder: 1,
            alternative_minimal_age_of_holder: Some(1),
            condition_for_alternative_minimal_age: Some(String::new()),
            disabled: 1,
        }
    }
}
