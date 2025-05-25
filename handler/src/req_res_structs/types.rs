use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonResponseType {
    pub id: i32,
    pub name: String
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonRequestType {
    pub id: Option<i32>,
    pub name: String
}

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct DeleteResponseType {
    pub rows: u64
}