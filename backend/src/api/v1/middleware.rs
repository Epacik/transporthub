use std::{future::{ready, Ready}, rc::Rc};
use actix_web::{dev::forward_ready, http::header::HeaderMap};
use actix_web::ResponseError;
use futures_util::future::LocalBoxFuture;

use actix_web::dev::{
        Service,
        Transform,
        ServiceRequest,
        ServiceResponse
    };

use crate::{errors::{self, ErrorResponse}, db_model::UserAccessKeys};

#[derive(Debug, Clone, Default)]
pub struct Auth;

impl<S: 'static> Transform<S, ServiceRequest> for Auth
where
    S: Service<ServiceRequest, Response = ServiceResponse, Error = actix_web::Error>,
    S::Future: 'static,
    {
        type Response = ServiceResponse;
        type Error = actix_web::Error;
        type Transform = AuthMiddleware<S>;
        type InitError = ();
        type Future = Ready<Result<Self::Transform, Self::InitError>>;

        fn new_transform(&self, service: S) -> Self::Future {
            ready(Ok(AuthMiddleware { service: Rc::new(service) }))
        }


}

pub struct AuthMiddleware<S> {
    service: Rc<S>,
}

impl<S> Service<ServiceRequest> for AuthMiddleware<S>
where
    S: Service<ServiceRequest, Response = ServiceResponse, Error = actix_web::Error> + 'static,
    S::Future: 'static,
    {

        type Response = S::Response;
        type Error = S::Error;
        type Future = LocalBoxFuture<'static, Result<Self::Response, Self::Error>>;

        forward_ready!(service);


        fn call(&self, req: ServiceRequest) -> Self::Future {
            let svc = self.service.clone();

            Box::pin(async move {
                if let Some(response) = cleanup_stale_sessions().await {
                    let response = req.into_response(response.error_response());
                    return Ok(response);
                }

                let ip = match super::get_ip_address(&req.request()) {
                    Ok(i) => i,
                    Err(_) => {
                        let response = req.into_response(errors::ip_unobtainable().error_response());
                        return Ok(response);
                    }
                };

                let path = req.path().to_string();

                if (!path.ends_with("check-connection")) && (!path.ends_with("auth/login")) {
                    if let Some(response) = authenticate(&req.headers(), &ip).await {
                        let response = req.into_response(response.error_response());
                        return Ok(response);
                    }
                }

                let res = svc.call(req).await?;
                Ok(res)
            })
        }
}


async fn cleanup_stale_sessions() -> Option<ErrorResponse> {
    let mut context = crate::db_model::context().await;

    if let Err(err) = super::cleanup_stale_sessions(&mut context).await {
        Some(errors::database_error(&err))
    }
    else {
        None
    }
}

async fn authenticate(headers: &HeaderMap, ip: &String) -> Option<ErrorResponse> {

    let (user, key) = match super::get_auth_header_info(&headers) {
        Err(err) => return Some(err),
        Ok(t) => t,
    };

    let mut context = crate::db_model::context().await;

    let access_key = match UserAccessKeys::select_by_key(&mut context, &key).await {
        Err(err) => return Some(errors::database_error(&err)),
        Ok(value) => value,
    };

    let access_key = match access_key {
        None => return Some(errors::auth::invalid_token()),
        Some(s) => s,
    };

    if access_key.user_id != (user as i32) || &(access_key.device_id) != ip {
        return Some(errors::auth::invalid_token());
    }
    else {
        None
    }
}
