use crate::AppState;
use crate::req_res_structs::menu_item::{CommonRequestMi, CommonResponseMi, DeletionResponseMi};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::{Json, debug_handler};
use entity::menu_item::{ActiveModel as MenuItemActiveModel, Entity as MenuItemEntity};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
use tracing::{error, instrument};
use utoipa::path as SwaggerAPIPath;
use utoipa_axum::router::OpenApiRouter;
use utoipa_axum::routes;

#[SwaggerAPIPath(
    get,
    path = "/",
    tag = "Menu Item Management",
    operation_id = "get_menu_item",
    responses(
        (status=200, body=Vec<CommonResponseMi>, description="Menu Item Object", example=json!({"id": 1, "name": "testing"})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseMi>>, (StatusCode, String)> {
    let result: Vec<CommonResponseMi> = MenuItemEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Retrieving menu item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponseMi {
            id: item.id,
            name: item.to_owned().name,
        })
        .collect();

    Ok(Json(result))
}

#[SwaggerAPIPath(
    post,
    path = "/",
    tag = "Menu Item Management",
    operation_id = "create_menu_item",
    request_body = CommonRequestMi,
    responses(
        (status=200, body=CommonRequestMi, description="Menu Item Object", example=json!({"id": 1, "name": "testing", "quantity": 1, "type":{"id": 1, "name": "type"}, "unit":{"id": 1, "name": "unit"}})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMi>,
) -> Result<Json<CommonResponseMi>, (StatusCode, String)> {
    let result = MenuItemActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Creating menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseMi {
        id: result.id,
        name: result.to_owned().name,
    }))
}

#[SwaggerAPIPath(
    put,
    path = "/",
    tag = "Menu Item Management",
    operation_id = "update_menu_item",
    request_body = CommonRequestMi,
    responses(
        (status=200, body=CommonRequestMi, description="Menu Item Object", example=json!({"id": 1, "name": "testing", "quantity": 1, "type":{"id": 1, "name": "type"}, "unit":{"id": 1, "name": "unit"}})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMi>,
) -> Result<Json<CommonResponseMi>, (StatusCode, String)> {
    let id = payload.id.ok_or_else(|| {
        (
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        )
    })?;

    let result = MenuItemActiveModel {
        id: Set(id),
        name: Set(payload.name.to_owned()),
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        error!("Updating menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        error!("Converting menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseMi {
        id: result.id,
        name: result.name.to_owned(),
    }))
}

#[SwaggerAPIPath(
    delete,
    path = "/{id}",
    tag = "Menu Item Management",
    operation_id = "delete_menu_item",
    params(("id", Path, description = "The id of menu item record")),
    responses(
        (status=200, body=DeletionResponseMi, description="Menu Item Object", example=json!({"rows": 1})),
        (status=500, body=String, description="Error message", example=json!("Failed"))
    )
)]
#[instrument]
#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeletionResponseMi>, (StatusCode, String)> {
    let result: DeleteResult = MenuItemEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            error!("Deleting menu item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeletionResponseMi {
        rows: result.rows_affected,
    }))
}

pub fn router() -> OpenApiRouter<AppState> {
    // Router::new()
    //     .route("/", get(read).post(creation).put(update))
    //     .route("/{id}", delete(deletion))

    OpenApiRouter::new().routes(routes!(read, creation, update, deletion))
}
