use serde::{Deserialize, Serialize};
use utoipa::ToSchema;
use crate::req_res_structs::types::CommonResponseType;
use crate::req_res_structs::unit::CommonResponseUnit;

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonResponseFoodItem {
    pub id: i32,
    pub name: String,
    pub quantity: i32,
    pub units: CommonResponseUnit,
    pub types: CommonResponseType
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct FoodItemModel {
    pub id: i32,
    pub name: String,
    pub quantity: i32,
    pub units: i32,
    pub types: i32
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonRequestFoodItem {
    pub id: Option<i32>,
    pub name: String,
    pub quantity: i32,
    pub unit_id: i32,
    pub type_id: i32
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct DeleteResponseFoodItem {
    pub rows: u64
}