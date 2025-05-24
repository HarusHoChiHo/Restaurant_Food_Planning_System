use sea_orm::prelude::DateTime;
use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonResponseOrder {
    pub id: i32,
    pub is_canceled: bool,
    pub order_date: DateTime,
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonRequestOrder {
    pub id: Option<i32>,
    pub is_canceled: bool
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct DeleteResponseOrder {
    pub rows: u64
}