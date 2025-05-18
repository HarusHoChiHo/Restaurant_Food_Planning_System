use crate::AppState;
use crate::req_res_structs::menu_item::{CommonRequestMi, CommonResponseMi, DeletionResponseMi};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router, debug_handler};
use entity::menu_item::{ActiveModel as MenuItemActiveModel, Entity as MenuItemEntity};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};
#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponseMi>>, (StatusCode, String)> {
    let result: Vec<CommonResponseMi> = MenuItemEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving menu item data error: {:?}", e);
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
        eprintln!("Creating menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseMi {
        id: result.id,
        name: result.to_owned().name,
    }))
}

#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequestMi>,
) -> Result<Json<CommonResponseMi>, (StatusCode, String)> {
    if payload.id.is_none() {
        return Err((
            StatusCode::INTERNAL_SERVER_ERROR,
            "id cannot be null.".to_string(),
        ));
    }

    let result = MenuItemActiveModel {
        id: Set(payload.id.unwrap()),
        name: Set(payload.name.to_owned()),
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Updating menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Converting menu item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(CommonResponseMi {
        id: result.id,
        name: result.name.to_owned(),
    }))
}

#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeletionResponseMi>, (StatusCode, String)> {
    let result: DeleteResult = MenuItemEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Deleting menu item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeletionResponseMi {
        rows: result.rows_affected,
    }))
}

pub fn router() -> Router<AppState> {
    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{id}", delete(deletion))
}
