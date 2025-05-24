use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonResponseOrderItem {
    pub id: i32,
    pub order_id: i32,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonRequestOrderItem {
    pub id: Option<i32>,
    pub order_id: i32,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct DeleteResponseOrderItem {
    pub rows: u64
}