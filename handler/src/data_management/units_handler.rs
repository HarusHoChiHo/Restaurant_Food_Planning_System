use axum::{Json, Router};
use axum::extract::State;
use axum::routing::post;
use sea_orm::{Database, DatabaseConnection, EntityTrait};
use crate::AppState;
use crate::structs::env::EnvironmentVariable;
use entity::units::{Entity as Units, Model};

async fn read(State(state): State<AppState>) -> Json<Vec<Model>> {
    let db: DatabaseConnection = Database::connect(state.env.database_url).await.unwrap();
    
    let result = Json(Units::find().all(&db).await.unwrap());
    
    db.close().await.unwrap();
    
    result
}

pub fn router() -> Router {
    let env = EnvironmentVariable::from_env().unwrap();
    let state = AppState { env };
    Router::new()
        .route("/read", post(read))
        .with_state(state)
}