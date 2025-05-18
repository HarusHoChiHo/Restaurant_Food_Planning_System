use serde::{Deserialize, Serialize};
use utoipa::{ToSchema};


#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonResponseUnit {
    pub id: i32,
    pub name: String
}

#[derive(Serialize, Deserialize, ToSchema)]
pub struct CommonRequestUnit {
    pub id: Option<i32>,
    pub name: String
}

#[derive(Serialize,Deserialize, ToSchema)]
pub struct DeleteResponseUnit {
    pub rows: u64
}