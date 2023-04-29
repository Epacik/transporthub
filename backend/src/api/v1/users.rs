use actix_web::{dev::HttpServiceFactory, HttpRequest, HttpResponse, get};

use super::Response;

pub fn scope() -> impl HttpServiceFactory {
    actix_web::web::scope("/users")
    .service(list)
}

#[get("/list")]
pub async fn list(req: HttpRequest) -> Response {
    let user_info = match super::get_user_info(&req) {
        Ok(val) => val,
        Err(err) => return Err(err),
    };



    Ok(HttpResponse::Ok().finish())
}