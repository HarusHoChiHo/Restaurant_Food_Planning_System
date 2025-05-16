use serde::{Deserialize, Serialize};
use entity::{menu_item::Model as MenuItemModel};
use entity::{food_item::Model as FoodItemModel};

#[derive(Serialize, Deserialize)]
pub struct CommonResponse {
    pub menu_item: MenuItemModel,
    pub food_item: FoodItemModel,
    pub consumption: f32,
}



#[derive(Serialize, Deserialize)]
pub struct CommonRequest {
    pub menu_item_id: i32,
    pub food_item_id: i32,
    pub consumption: f32
}

#[derive(Serialize, Deserialize)]
pub struct DeleteResponse {
    pub rows: u64,
}
