use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonResponseType {
    pub id: i32,
    pub name: String
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonRequestType {
    pub id: Option<i32>,
    pub name: String
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct DeleteResponseType {
    pub rows: u64
}