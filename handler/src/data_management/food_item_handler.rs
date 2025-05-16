use crate::AppState;
use crate::req_res_structs::food_item::{CommonResponse, Creation, Update};
use crate::req_res_structs::unit::DeleteResponse;
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router, debug_handler};
use entity::food_item::{
    ActiveModel as FoodItemActiveModel, Entity as FoodItemEntity, Model as FoodItemModel,
};
use entity::types::{Entity as TypeEntity, Model as TypeModel};
use entity::units::{Entity as UnitEntity, Model as UnitModel};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, ModelTrait, Set, TryIntoModel};

#[debug_handler]
async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponse>>, (StatusCode, String)> {
    let result: Vec<CommonResponse> = FoodItemEntity::find()
        .find_also_related(TypeEntity)
        .find_also_related(UnitEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponse {
            id: item.0.id,
            name: item.0.name.clone(),
            quantity: item.0.quantity.clone(),
            types: item.to_owned().1.unwrap(),
            units: item.to_owned().2.unwrap(),
        })
        .collect();

    Ok(Json(result))
}

#[debug_handler]
async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<Creation>,
) -> Result<Json<CommonResponse>, (StatusCode, String)> {
    let result = FoodItemActiveModel {
        name: Set(payload.name),
        quantity: Set(payload.quantity),
        type_id: Set(payload.type_id),
        unit_id: Set(payload.unit_id),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Creating food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        println!("Converting food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let types_model: TypeModel = result
        .find_related(TypeEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Find type related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .get(0)
        .unwrap()
        .to_owned();

    let units_model: UnitModel = result
        .find_related(UnitEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Find unit related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .get(0)
        .unwrap()
        .to_owned();

    let response = CommonResponse {
        id: result.id,
        name: result.name,
        quantity: result.quantity,
        types: types_model,
        units: units_model,
    };

    Ok(Json(response))
}

#[debug_handler]
async fn update(
    State(state): State<AppState>,
    Json(payload): Json<Update>,
) -> Result<Json<CommonResponse>, (StatusCode, String)> {
    let result: FoodItemModel = FoodItemActiveModel {
        id: Set(payload.id),
        name: Set(payload.name),
        quantity: Set(payload.quantity),
        type_id: Set(payload.type_id),
        unit_id: Set(payload.unit_id),
        ..Default::default()
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Updating food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Converting food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let types_model: TypeModel = result
        .find_related(TypeEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Find type related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .get(0)
        .ok_or("Failed to unwrapping value of types")
        .map_err(|e| {
            eprintln!("{}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .to_owned();

    let units_model: UnitModel = result
        .find_related(UnitEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Find unit related to food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .get(0)
        .ok_or("Failed to unwrapping value of units")
        .map_err(|e| {
            eprintln!("{}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .to_owned();

    let response = CommonResponse {
        id: result.id,
        name: result.name,
        quantity: result.quantity,
        types: types_model,
        units: units_model,
    };

    Ok(Json(response))
}

#[debug_handler]
async fn deletion(
    State(state): State<AppState>,
    Path(id): Path<i32>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let result: DeleteResult = FoodItemEntity::delete_by_id(id)
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Deleting food item data error: {:?}", e);
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
