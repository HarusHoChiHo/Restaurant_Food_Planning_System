use rust_decimal::Decimal;
use serde::{Deserialize, Serialize};
use utoipa::ToSchema;
use crate::req_res_structs::food_item::FoodItemModel;
use crate::req_res_structs::menu_item::CommonResponseMi;

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonResponseMiFi {
    pub menu_item: CommonResponseMi,
    pub food_item: FoodItemModel,
    pub consumption: Decimal,
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonRequestMiFi {
    pub menu_item_id: i32,
    pub food_item_id: i32,
    pub consumption: Decimal
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct DeleteResponseMiFi {
    pub rows: u64,
}
