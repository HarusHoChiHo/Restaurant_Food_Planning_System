use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct CommonResponseMi {
    pub id: i32,
    pub name: String,
}
#[derive(Serialize, Deserialize)]
pub struct CommonRequestMi {
    pub id: Option<i32>,
    pub name: String,
}
#[derive(Serialize, Deserialize)]
pub struct DeletionResponseMi {
    pub rows: u64,
}
