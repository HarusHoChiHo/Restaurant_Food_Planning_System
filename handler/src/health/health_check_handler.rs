use crate::AppState;
use crate::req_res_structs::health_check::HealthResponse;
use axum::{debug_handler, Json};
use axum::response::IntoResponse;
use tracing::{info, instrument};
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[utoipa::path(
    get,
    path = "/",
    responses(
        (status = 200, description = "API is healthy", body = HealthResponse)
    )
)]
#[instrument]
#[debug_handler]
async fn health_check() -> impl IntoResponse {
    info!("health_check is called.");
    Json(HealthResponse {
        status: "ok".to_string(),
    })
}

pub fn router() -> OpenApiRouter<AppState> {
    OpenApiRouter::new().routes(routes!(health_check))
}
