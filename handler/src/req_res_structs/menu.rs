use sea_orm::prelude::Date;
use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonResponseMenu {
    pub id: i32,
    pub date: Date,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonRequestMenu {
    pub id: Option<i32>,
    pub date: Date,
    pub menu_item_id: i32
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct DeleteResponseMenu {
    pub rows: u64
}