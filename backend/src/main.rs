extern crate argonautica;
extern crate base64;
extern crate num_derive;
use std::time::Duration;

use actix_cors::Cors;
use actix_web::{HttpServer, App, middleware, http};
use pkg_version::*;

pub mod db_model;
pub mod config;
pub mod errors;
mod api;


const MAJOR: u32 = pkg_version_major!();
const MINOR: u32 = pkg_version_minor!();
const PATCH: u32 = pkg_version_patch!();


#[actix_web::main]
async fn main() -> std::io::Result<()> {
    fast_log::init(
        fast_log::Config::new()
        .console()
        .file_split(
            "target/test.log",
            fast_log::consts::LogSize::MB(250),
            fast_log::plugin::file_split::RollingType::KeepTime(Duration::from_secs(1 * 60 * 60)), // 1h
            fast_log::plugin::packer::LogPacker{})
        .chan_len(Some(100000))
        .level(log::LevelFilter::Trace)
    ).unwrap();

    log::info!("Initialized logging system");
    log::info!("Initializing Actix-web");

    HttpServer::new(move ||

        App::new()
            .wrap(middleware::Logger::default())
            .wrap(middleware::Logger::new("%a %{User-Agent}i"))
            .wrap(middleware::NormalizePath::default())
            .wrap(Cors::default()
                .allow_any_origin()
                .send_wildcard()
                .allowed_methods(vec!["GET", "POST", "PUT", "DELETE", "PATCH"])
                .allowed_headers(vec![
                    http::header::AUTHORIZATION,
                    http::header::ACCEPT,
                    http::header::CONTENT_TYPE])
                .max_age(3600))
            .wrap(middleware::DefaultHeaders::new()
                .add(("X-TransportHub-Version", format!("{MAJOR}.{MINOR}.{PATCH}"))))
            .service(api::scope())
        )
        .bind(("0.0.0.0", 8080))?
        .run()
        .await

}




