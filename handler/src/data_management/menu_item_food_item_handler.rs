use crate::AppState;
use crate::req_res_structs::menu_item_food_item::CommonResponse;
use axum::extract::State;
use axum::http::StatusCode;
use axum::{Json, Router, debug_handler};
use axum::routing::get;
use entity::menu_item_food_item::{
    ActiveModel as MenuItemFoodItemActiveEntity, Entity as MenuItemFoodItemEntity,
    Model as MenuItemFoodItemModel,
};
use entity::{menu_item::Entity as MenuItemEntity};
use entity::{food_item::Entity as FoodItemEntity};
use sea_orm::EntityTrait;

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
        .map(|item| {
            CommonResponse {
                menu_item: item.1.clone().unwrap(),
                food_item: item.2.clone().unwrap(),
                consumption: item.0.consumption
            }
        })
        .collect();

    Ok(Json(result))
}

#[debug_handler]
pub async fn creation() {
    todo!()
}

#[debug_handler]
pub async fn update() {
    todo!()
}

#[debug_handler]
pub async fn deletion() {
    todo!()
}

pub fn router() -> Router<AppState> {
    Router::new()
        .route("/", get(read))
}
