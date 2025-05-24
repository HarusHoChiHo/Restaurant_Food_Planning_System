use crate::AppState;
use crate::req_res_structs::order_item::{
    CommonRequestOrderItem, CommonResponseOrderItem, DeleteResponseOrderItem,
};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json};
use entity::order_item::{
    ActiveModel as OrderItemActiveModel, Entity as OrderItemEntity, Model as OrderItemModel,
};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Order Item Management",
    operation_id = "get_order_item",
    responses(
        (status=200, body=Vec<CommonResponseOrderItem>, description="Unit Object", example=json!({"id": 1, "order_id": 1, "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseOrderItem>>, (StatusCode, String)> {
    let result = OrderItemEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving order item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseOrderItem {
            id: item.id,
            order_id: item.order_id,
            menu_item_id: item.menu_item_id,
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Order Item Management",
    operation_id = "create_order_item",
    request_body = CommonRequestOrderItem,
    responses(
        (status=200, body=CommonResponseOrderItem, description="Order Item Object", example=json!({"id": 1, "order_id": 1, "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestOrderItem>,
) -> Result<Json<CommonResponseOrderItem>, (StatusCode, String)> {
    let insert_result: OrderItemModel = OrderItemActiveModel {
        order_id: Set(payload.order_id),
        menu_item_id: Set(payload.menu_item_id),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Database save error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Database save error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseOrderItem {
        id: insert_result.id,
        order_id: insert_result.order_id,
        menu_item_id: insert_result.menu_item_id,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Order Item Management",
    operation_id = "update_order_item",
    request_body = CommonRequestOrderItem,
    responses(
        (status=200, body=CommonResponseOrderItem, description="Order Item Object", example=json!({"id": 1, "order_id": 1, "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestOrderItem>,
) -> Result<Json<CommonResponseOrderItem>, (StatusCode, String)> {
    if payload.id.is_none() {
        return Err((
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        ));
    }

    let active_model: OrderItemActiveModel = OrderItemActiveModel {
        id: Set(payload.id.unwrap()),
        order_id: Set(payload.order_id),
        menu_item_id: Set(payload.menu_item_id),
        ..Default::default()
    };

    let update_result: OrderItemModel = active_model
        .save(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Database save error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .try_into_model()
        .map_err(|e| {
            eprintln!("Database convert error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(CommonResponseOrderItem {
        id: update_result.id,
        order_id: update_result.order_id,
        menu_item_id: update_result.menu_item_id,
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Order Item Management",
    operation_id = "delete_order_item",
    params(("id", Path, description = "The id of unit record")),
    responses(
        (status=200, body=DeleteResponseOrderItem, description="Order Item Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[axum::debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseOrderItem>, (StatusCode, String)> {
    let result: DeleteResult = OrderItemEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .unwrap();

    Ok(Json(DeleteResponseOrderItem {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}
