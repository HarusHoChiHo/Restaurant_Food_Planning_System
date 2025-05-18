use crate::AppState;
use crate::req_res_structs::menu_item_food_item::{CommonRequest, CommonResponse, DeleteResponse};
use axum::extract::{Path, State};
use axum::http::StatusCode;
use axum::routing::{delete, get};
use axum::{Json, Router, debug_handler};
use entity::food_item::{Entity as FoodItemEntity};
use entity::menu_item::{Entity as MenuItemEntity};
use entity::menu_item_food_item::{
    ActiveModel as MenuItemFoodItemActiveModel, Entity as MenuItemFoodItemEntity,
    Model as MenuItemFoodItemModel,
};
use sea_orm::{ActiveModelTrait, DeleteResult, EntityTrait, Set, TryIntoModel};

#[debug_handler]
pub async fn read(
    State(state): State<AppState>,
) -> Result<Json<Vec<CommonResponse>>, (StatusCode, String)> {
    let result: Vec<CommonResponse> = MenuItemFoodItemEntity::find()
        .find_also_related(MenuItemEntity)
        .find_also_related(FoodItemEntity)
        .all(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Retrieving menu item food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?
        .iter()
        .map(|item| CommonResponse {
            menu_item: item.1.clone().unwrap(),
            food_item: item.2.clone().unwrap(),
            consumption: item.0.consumption,
        })
        .collect();

    Ok(Json(result))
}

#[debug_handler]
pub async fn creation(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequest>,
) -> Result<Json<CommonResponse>, (StatusCode, String)> {
    eprintln!("{:?}", payload);

    let result: MenuItemFoodItemModel = MenuItemFoodItemActiveModel {
        consumption: Set(payload.consumption),
        menu_item_id: Set(payload.menu_item_id),
        food_item_id: Set(payload.food_item_id),
        ..Default::default()
    }
    .insert(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Creating menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .try_into_model()
    .map_err(|e| {
        eprintln!("Converting menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let (mifi, fi, mi) =
        MenuItemFoodItemEntity::find_by_id((result.menu_item_id, result.food_item_id))
            .find_also_related(FoodItemEntity)
            .find_also_related(MenuItemEntity)
            .one(&state.env.database_connection)
            .await
            .map_err(|e| {
                eprintln!("Retrieving menu item food item data error: {:?}", e);
                (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
            })?
            .unwrap();

    Ok(Json(CommonResponse {
        food_item: fi.unwrap(),
        menu_item: mi.unwrap(),
        consumption: mifi.consumption,
    }))
}

#[debug_handler]
pub async fn update(
    State(state): State<AppState>,
    Json(payload): Json<CommonRequest>,
) -> Result<Json<CommonResponse>, (StatusCode, String)> {
    let result: MenuItemFoodItemActiveModel = MenuItemFoodItemActiveModel {
        menu_item_id: Set(payload.menu_item_id),
        food_item_id: Set(payload.food_item_id),
        consumption: Set(payload.consumption),
    }
    .save(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Updating menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;

    let (mifi, fi, mi) = MenuItemFoodItemEntity::find_by_id((
        result.food_item_id.unwrap(),
        result.menu_item_id.unwrap(),
    ))
    .find_also_related(FoodItemEntity)
    .find_also_related(MenuItemEntity)
    .one(&state.env.database_connection)
    .await
    .map_err(|e| {
        eprintln!("Retrieving menu item food item data error: {:?}", e);
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?
    .unwrap();

    Ok(Json(CommonResponse {
        consumption: mifi.consumption,
        menu_item: mi.unwrap(),
        food_item: fi.unwrap(),
    }))
}

#[debug_handler]
pub async fn deletion(
    State(state): State<AppState>,
    Path((menu_item_id, food_item_id)): Path<(i32, i32)>,
) -> Result<Json<DeleteResponse>, (StatusCode, String)> {
    let result: DeleteResult = MenuItemFoodItemEntity::delete_by_id((menu_item_id, food_item_id))
        .exec(&state.env.database_connection)
        .await
        .map_err(|e| {
            eprintln!("Deleting menu item food item data error: {:?}", e);
            (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
        })?;

    Ok(Json(DeleteResponse {
        rows: result.rows_affected,
    }))
}

pub fn router() -> Router<AppState> {
    Router::new()
        .route("/", get(read).post(creation).put(update))
        .route("/{mid}/{fid}", delete(deletion))
}
