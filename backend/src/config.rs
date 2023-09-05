use std::{path::PathBuf, str::FromStr};

use actix_web::{web::JsonConfig, HttpResponse};
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
    database_info: DatabaseConnectionInfo,
}

#[derive(Deserialize)]
enum DatabaseConnectionInfo {
    PostgresInfo(PostgresInfo),
    SqliteInfo(String),
}

#[derive(Deserialize)]
struct PostgresInfo {
    pub address: String,
    pub database: String,
    pub username: String,
    pub password: String,
    pub port: Option<String>,
}

pub enum DatabaseType {
    PostgreSQL,
    Sqlite,
}

impl Config {
    pub fn pass_secret(&self) -> &str {
        self.pass_secret.as_ref()
    }

    pub fn connection_string(&self) -> String {

        match &self.database_info {
            DatabaseConnectionInfo::PostgresInfo(p) => {
                let port = p.port.clone().unwrap_or_else(|| String::from("5432"));
                format!(
                    "postgres://{3}:{4}@{0}:{1}/{2}",
                    p.address,
                    port,
                    p.database,
                    p.username,
                    p.password
                )
            },
            DatabaseConnectionInfo::SqliteInfo(s) => format!("sqlite://{}/{}", (&CONFIG_DIR).to_string(), s),
        }
    }

    pub fn used_database(&self) -> DatabaseType {
        match self.database_info {
            DatabaseConnectionInfo::PostgresInfo(_) => DatabaseType::PostgreSQL,
            DatabaseConnectionInfo::SqliteInfo(_) => DatabaseType::Sqlite,
        }
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
