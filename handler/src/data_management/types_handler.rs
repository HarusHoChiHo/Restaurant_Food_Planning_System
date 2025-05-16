use crate::AppState;
use crate::req_res_structs::types::{Creation, DeleteResponse, Update};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router, debug_handler};
use entity::types::{ActiveModel as TypesActiveModel, Entity as TypesEntity, Model as TypesModel};
use sea_orm::{
    ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel,
};

#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<TypesModel>>, (StatusCode, String)> {
    let result: Vec<TypesModel> = TypesEntity::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving types data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(result))
}

#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<Creation>,
) -> Result<Json<TypesModel>, (StatusCode, String)> {
    let types: TypesModel = TypesActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Saving types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Converting types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(types))
}

#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<Update>,
) -> Result<Json<TypesModel>, (StatusCode, String)> {
    let result: TypesModel = TypesActiveModel {
        id: Set(payload.id),
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Update types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Converting types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(result))
}

#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let result: DeleteResult = TypesEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Deleting types data by id error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponse {
        rows: result.rows_affected,
    }))
}

pub fn router() -> Router<AppState> {
    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{id}", delete(deletion))
}
