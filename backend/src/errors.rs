use actix_web::{
    error,
    http::{header::ContentType, StatusCode},
    HttpResponse,
};

lazy_static::lazy_static!{
    static ref DEBUG: bool = {
        let raw = match std::env::var("TRANSPORTIE_DEBUG") {
            Ok(val) => val,
            Err(_) => String::new(),
        };

        raw.eq_ignore_ascii_case("true")
    };
}

#[derive(Debug, derive_more::Display, derive_more::Error)]
#[display(fmt = "ErrorResponse ({0}): {1}", status_code, body)]
pub struct ErrorResponse {
    status_code: StatusCode,
    body: String,
}

impl error::ResponseError for ErrorResponse {
    fn error_response(&self) -> HttpResponse {
        HttpResponse::build(self.status_code())
            .insert_header(ContentType::html())
            .body(self.body.clone())
    }

    fn status_code(&self) -> StatusCode {
        self.status_code
    }
}

impl ErrorResponse {
    pub fn new<S: AsRef<str>>(status_code: StatusCode, body: S) -> ErrorResponse {
        let body = body.as_ref().to_string();
        ErrorResponse { status_code, body }
    }
}

pub fn database_error(err: &rbatis::rbdc::Error) -> ErrorResponse {

    log::error!("An error occured while retreiving data from a database\n{0}", err);


    let body = if *DEBUG { format!("Internal server error:\n {0}", err) } else { String::from("Internal server error") };

    ErrorResponse::new(StatusCode::INTERNAL_SERVER_ERROR, body)
}

pub fn ip_unobtainable() -> ErrorResponse {

    let body = if *DEBUG {
        "Cannot obtain IP address"
    }
    else {
        "Cannot obtain IP address"
    };

    ErrorResponse::new(StatusCode::INTERNAL_SERVER_ERROR, body)
}

pub(crate) fn invalid_user() -> ErrorResponse {
    ErrorResponse::new(StatusCode::INTERNAL_SERVER_ERROR, "unknown user")
}

pub(crate) fn insufficient_privileges(user_id: i32) -> ErrorResponse {
    let body = if *DEBUG {
        format!("User with id `{0}` has insufficient privileges to perform this action", user_id)
    }
    else {
        String::from("User has insufficient privileges to perform this action")
    };

    ErrorResponse::new(StatusCode::FORBIDDEN, body)
}

pub(crate) fn hashing_error(err: &argonautica::Error) -> ErrorResponse {
    let body = if *DEBUG {
        format!("Hashing password returned an error\n {0}", err)
    }
    else {
        String::from("Internal server error")
    };

    ErrorResponse::new(StatusCode::INTERNAL_SERVER_ERROR, body)
}


pub mod auth {
    use actix_web::http::StatusCode;

    use super::ErrorResponse;

    use super::DEBUG;

    pub fn invalid_username() -> ErrorResponse {
        log::info!("User was not found");

        let body = if *DEBUG { "Invalid username" } else { "Invalid username or password" };

        ErrorResponse::new(StatusCode::UNAUTHORIZED, body)
    }

    pub fn invalid_password() -> ErrorResponse {
        log::info!("Invalid password");

        let body = if *DEBUG { "Invalid password" } else { "Invalid username or password" };

        ErrorResponse::new(StatusCode::UNAUTHORIZED, body)
    }

    pub fn password_expired() -> ErrorResponse {
        log::info!("Password has expired");
        ErrorResponse::new(StatusCode::UNAUTHORIZED, "Password has expired")
    }

    pub fn multi_login_not_allowed() -> ErrorResponse {
        log::info!("This user cannot have multiple sessions");
        ErrorResponse::new(StatusCode::UNAUTHORIZED, "This user cannot have multiple active sessions")
    }

    pub(crate) fn invalid_token() -> ErrorResponse {
        ErrorResponse::new(StatusCode::FORBIDDEN, "Provided token is invalid")
    }

    pub(crate) fn invalid_token_info(err: &dyn std::error::Error) -> ErrorResponse {
        log::info!("Invalid token\n {0}", err);
        let body = if *DEBUG {
            format!("Provided token is invalid:\n {0}", err)
        }
        else {
            String::from("Provided token is invalid")
        };

        ErrorResponse::new(StatusCode::FORBIDDEN, body)
    }
}

pub mod users {
    use actix_web::http::StatusCode;

    use super::{ErrorResponse, DEBUG};

    pub(crate) fn already_exists() -> ErrorResponse {
        ErrorResponse::new(StatusCode::CONFLICT, "User already exists")
    }

    pub(crate) fn creation_error() -> ErrorResponse {
        ErrorResponse::new(StatusCode::INTERNAL_SERVER_ERROR, if *DEBUG { "Problem creading user" } else { "Internal server error" })
    }


}


