use crate::AppState;
use crate::structs::env::EnvironmentVariable;
use crate::structs::unit::{Creation, DeleteResponse, Update};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router, debug_handler};
use entity::types::{ActiveModel as TypesActiveModel, Entity as TypesEntity, Model as TypesModel};
use sea_orm::{
    ActiveModelTrait, Database, DatabaseConnection, DeleteResult, EntityTrait, Set, TryIntoModel,
};

#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<TypesModel>>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database connection error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    let result: Vec<TypesModel> = TypesEntity::find().all(&db).await.map_err(|e| {
        eprintln!("Retrieving types data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    db.close().await.map_err(|e| {
        eprintln!("Database close error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(result))
}

#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<Creation>,
) -> Result<Json<TypesModel>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database connection error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    let types: TypesModel = TypesActiveModel {
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&db)
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

    db.close().await.map_err(|e| {
        eprintln!("Database connection close error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(types))
}

#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<Update>,
) -> Result<Json<TypesModel>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database connection error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    let result: TypesModel = TypesActiveModel {
        id: Set(payload.id),
        name: Set(payload.name),
        ..Default::default()
    }
    .save(&db)
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

    db.close().await.map_err(|e| {
        eprintln!("Database connection close error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(result))
}

#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let db: DatabaseConnection = Database::connect(state.env.database_url)
        .await
        .map_err(|e| {
            eprintln!("Database connection error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    let result: DeleteResult = TypesEntity::delete_by_id(id).exec(&db).await.map_err(|e| {
        eprintln!("Deleting types data by id error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    db.close().await.map_err(|e| {
        eprintln!("Database connection close error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    Ok(Json(DeleteResponse {
        rows: result.rows_affected,
    }))
}

pub fn router() -> Router {
    let env = EnvironmentVariable::from_env().unwrap();
    let state = AppState { env };

    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{id}", delete(deletion))
        .with_state(state)
}
