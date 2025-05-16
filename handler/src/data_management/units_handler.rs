use crate::AppState;
use crate::req_res_structs::unit;
use crate::req_res_structs::unit::DeleteResponse;
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router};
use entity::units::{ActiveModel, Entity as Units, Model as UnitsModel};
use sea_orm::{
    ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel,
};

#[axum::debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<UnitsModel>>, (StatusCode, String)> {
    let result = Units::find()
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving unit data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(result))
}

#[axum::debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<unit::Creation>,
) -> Result<Json<UnitsModel>, (StatusCode, String)> {
    let insert_result: UnitsModel = ActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&state.env.database_connection)
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

    Ok(Json(insert_result))
}

#[axum::debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<unit::Update>,
) -> Result<Json<UnitsModel>, (StatusCode, String)> {
    let active_model: ActiveModel = ActiveModel {
        id: Set(payload.id),
        name: Set(payload.name),
        ..Default::default()
    };

    let update_result: UnitsModel = active_model
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

    Ok(Json(update_result))
}

#[axum::debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let result: DeleteResult = Units::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .unwrap();

    Ok(Json(DeleteResponse {
        rows: result.rows_affected,
    }))
}

pub fn router() -> Router<AppState> {
    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{id}", delete(deletion))
}
