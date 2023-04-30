extern crate argonautica;
extern crate base64;
extern crate num_derive;
use std::time::Duration;

use actix_web::{HttpServer, App};

pub mod db_model;
pub mod config;
pub mod errors;
mod api;

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

    HttpServer::new(||
        App::new()
            .wrap(actix_web::middleware::Logger::default())
            .wrap(actix_web::middleware::Logger::new("%a %{User-Agent}i"))
            .wrap(actix_web::middleware::NormalizePath::default())
            .service(api::scope())
        )
        .bind(("0.0.0.0", 8080))?
        .run()
        .await

}




