use std::{path::PathBuf, str::FromStr};

use actix_web::{web::{JsonConfig}, HttpResponse};
use serde::Deserialize;


lazy_static::lazy_static!{
    static ref CONFIG_DIR: String = {
        let config_path = match std::env::var("TRANSPORTIE_CONFIG_DIR") {
            Ok(val) => val,
            Err(_) => std::env::current_dir()
                .expect("Could not find config directory")
                .to_str().unwrap_or_default()
                .to_string(),
        };

        config_path
    };

    static ref CONFIG: Config = {
        let mut path = PathBuf::from_str(CONFIG_DIR.as_str()).unwrap();
        path.extend(&["config.json"]);
        let content = std::fs::read_to_string(&path)
            .expect(format!("Could not read the config file\nFile path: {0}", path.to_str().unwrap_or_default()).as_str());

        serde_json::from_str(content.as_str())
            .expect("Could not parse config file")
    };
}

pub fn config() -> &'static Config {
    &CONFIG
}

#[derive(Deserialize)]
pub struct Config {
    pass_secret: String,
    hash_ids_secret: String,
    database_info: DatabaseInfo,
}

#[derive(Deserialize)]
struct DatabaseInfo {
    pub address: String,
    pub database: String,
    pub username: String,
    pub password: String,
    pub port: Option<String>,
}

impl Config {
    pub fn pass_secret(&self) -> &str {
        self.pass_secret.as_ref()
    }

    pub(crate) fn connection_string(&self) -> String {
        let port = self.database_info.port.clone().unwrap_or_else(|| String::from("5432"));
        format!(
            "postgres://{3}:{4}@{0}:{1}/{2}",
            self.database_info.address,
            port,
            self.database_info.database,
            self.database_info.username,
            self.database_info.password
        )
    }

    pub fn hash_ids_secret(&self) -> &str {
        self.hash_ids_secret.as_ref()
    }
}


pub fn json_config() -> JsonConfig {
    JsonConfig::default()
        .limit(4096)
        .error_handler(|err, _req| {
            // create custom error response
            actix_web::error::InternalError::from_response(err, HttpResponse::Conflict().finish())
                .into()
        })
}