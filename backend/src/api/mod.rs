use actix_web::dev::HttpServiceFactory;

pub mod v1;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/api")
        .service(v1::scope())
}