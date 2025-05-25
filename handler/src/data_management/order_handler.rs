use crate::AppState;
use crate::req_res_structs::order::{CommonRequestOrder, CommonResponseOrder, DeleteResponseOrder};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::order::{ActiveModel as OrderActiveModel, Entity as OrderEntity, Model as OrderModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use sea_orm::sqlx::types::chrono::Local;
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Order Management",
    operation_id = "get_order",
    responses(
        (status=200, body=Vec<CommonResponseOrder>, description="Order Object", example=json!({"id": 1, "is_canceled": "false", "order_date":"2025-05-24 00:00:00"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseOrder>>, (StatusCode, String)> {
    let result: Vec<CommonResponseOrder> = OrderEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving order data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseOrder {
            id: item.id,
            is_canceled: item.is_canceled,
            order_date: item.order_date,
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Order Management",
    operation_id = "create_order",
    request_body = CommonRequestOrder,
    responses(
        (status=200, body=CommonResponseOrder, description="Order Object", example=json!({"id": 1, "is_canceled": "false", "order_date":"2025-05-24 00:00:00"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestOrder>,
) -> Result<Json<CommonResponseOrder>, (StatusCode, String)> {

    let orders: OrderModel = OrderActiveModel {
        is_canceled: Set(payload.is_canceled),
        order_date: Set(Local::now().naive_local()),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Saving order data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting order data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseOrder {
        id: orders.id,
        is_canceled: orders.is_canceled,
        order_date: orders.order_date,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Order Management",
    operation_id = "update_order",
    request_body = CommonRequestOrder,
    responses(
        (status=200, body=CommonResponseOrder, description="Order Object", example=json!({"id": 1, "is_canceled": "false", "order_date":"2025-05-24 00:00:00"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestOrder>,
) -> Result<Json<CommonResponseOrder>, (StatusCode, String)> {
    let id = payload.id.ok_or_else(|| {
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        )
    })?;

    let result: OrderModel = OrderActiveModel {
        id: Set(id),
        is_canceled: Set(payload.is_canceled),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Update order data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting order data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseOrder {
        id: result.id,
        is_canceled: result.is_canceled,
        order_date: result.order_date,
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Order Management",
    operation_id = "delete_order",
    params(("id", Path, description = "The id of order record")),
    responses(
        (status=200, body=DeleteResponseOrder, description="Order Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseOrder>, (StatusCode, String)> {
    let result: DeleteResult = OrderEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting types data by id error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponseOrder {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}
