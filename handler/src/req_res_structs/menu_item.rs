use serde::{Deserialize, Serialize};
use utoipa::ToSchema;

#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonResponseMi {
    pub id: i32,
    pub name: String,
}
#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct CommonRequestMi {
    pub id: Option<i32>,
    pub name: String,
}
#[derive(Serialize, Deserialize, Debug, ToSchema)]
pub struct DeletionResponseMi {
    pub rows: u64,
}
