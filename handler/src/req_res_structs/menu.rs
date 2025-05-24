use sea_orm::prelude::Date;
use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonResponseMenu {
    pub id: i32,
    pub date: Date,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonRequestMenu {
    pub id: Option<i32>,
    pub date: Date,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct DeleteResponseMenu {
    pub rows: u64
}