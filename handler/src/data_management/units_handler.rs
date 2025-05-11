use crate::AppState;
use crate::structs::env::EnvironmentVariable;
use crate::structs::unit;
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router};
use entity::units::{ActiveModel, Entity as Units, Model as UnitsModel};
use sea_orm::{
    ActiveModelTrait, Database, DatabaseConnection, DeleteResult, EntityTrait, ModelTrait, Set,
    TryIntoModel,
};
use crate::structs::unit::DeleteResponse;

#[axum::debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<UnitsModel>>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| (StatusCode::INTERNAL_SERVER_ERROR, e.to_string()))?;

    let result = Units::find().all(&db).await.map_err(|e| {
        eprintln!("Retrieving unit data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    db.close().await.map_err(|e| {
        eprintln!("Database save error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(result))
}

#[axum::debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<unit::Creation>,
) -> Result<Json<UnitsModel>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| (StatusCode::INTERNAL_SERVER_ERROR, e.to_string()))?;

    let insert_result: UnitsModel = ActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&db)
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

    db.close().await.map_err(|e| {
        eprintln!("Database connection close error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;
    
    Ok(Json(insert_result))
}

#[axum::debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<unit::Update>,
) -> Result<Json<UnitsModel>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database save error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    let active_model: ActiveModel = ActiveModel {
        id: Set(payload.id),
        name: Set(payload.name),
        ..Default::default()
    };

    let update_result: UnitsModel = active_model
        .save(&db)
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

    db.close().await.map_err(|e| {
        eprintln!("Database close error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(update_result))
}

#[axum::debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database connection error: {}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;
    
    let result: DeleteResult = Units::delete_by_id(id).exec(&db).await.unwrap();

    db.close().await.map_err(|e| {
        eprintln!("Database close error: {}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(DeleteResponse { rows: result.rows_affected }))
}

pub fn router() -> Router {
    let env = EnvironmentVariable::from_env().unwrap();
    let state = AppState { env };
    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{id}", delete(deletion))
        .with_state(state)
}
