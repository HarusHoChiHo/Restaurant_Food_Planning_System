use serde::{Deserialize, Serialize};

#[derive(Serialize,Deserialize)]
pub struct Creation {
    pub name: String,
}

#[derive(Serialize,Deserialize)]
pub struct Update {
    pub id: i32,
    pub name: String
}

#[derive(Serialize,Deserialize)]
pub struct DeleteResponse {
    pub rows: u64
}