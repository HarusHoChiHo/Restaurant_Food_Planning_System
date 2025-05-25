use crate::AppState;
use crate::req_res_structs::menu::{CommonRequestMenu, CommonResponseMenu, DeleteResponseMenu};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::menu::{ActiveModel as MenuActiveModel, Entity as MenuEntity, Model as MenuModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;
#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Menu Management",
    operation_id = "get_menu",
    responses(
        (status=200, body=Vec<CommonResponseMenu>, description="Menu Object", example=json!({"id": 1, "date":"2025-05-24 00:00:00", "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseMenu>>, (StatusCode, String)> {
    let result: Vec<CommonResponseMenu> = MenuEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving order data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseMenu {
            id: item.id,
            date: item.date,
            menu_item_id: item.menu_item_id,
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Menu Management",
    operation_id = "create_menu",
    request_body = CommonRequestMenu,
    responses(
        (status=200, body=CommonResponseMenu, description="Menu Object", example=json!({"id": 1, "date":"2025-05-24 00:00:00", "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMenu>,
) -> Result<Json<CommonResponseMenu>, (StatusCode, String)> {
    let menus: MenuModel = MenuActiveModel {
        date: Set(payload.date),
        menu_item_id: Set(payload.menu_item_id),
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

    Ok(Json(CommonResponseMenu {
        id: menus.id,
        date: menus.date,
        menu_item_id: menus.menu_item_id,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Menu Management",
    operation_id = "update_menu",
    request_body = CommonRequestMenu,
    responses(
        (status=200, body=CommonResponseMenu, description="Menu Object", example=json!({"id": 1, "date":"2025-05-24 00:00:00", "menu_item_id": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMenu>,
) -> Result<Json<CommonResponseMenu>, (StatusCode, String)> {
    let id = payload.id.ok_or_else(|| {
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        )
    })?;

    let result: MenuModel = MenuActiveModel {
        id: Set(id),
        date: Set(payload.date),
        menu_item_id: Set(payload.menu_item_id),
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

    Ok(Json(CommonResponseMenu {
        id: result.id,
        date: result.date,
        menu_item_id: result.menu_item_id,
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Menu Management",
    operation_id = "delete_menu",
    params(("id", Path, description = "The id of order record")),
    responses(
        (status=200, body=DeleteResponseMenu, description="Menu Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponseMenu>, (StatusCode, String)> {
    let result: DeleteResult = MenuEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting types data by id error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponseMenu {
        rows: result.rows_affected,
    }))
}
pub fn router() -> OpenApiRouter<AppState> {
    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}
