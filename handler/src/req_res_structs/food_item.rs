use serde::{Deserialize, Serialize};
use entity::units::Model as UnitModel;
use entity::types::Model as TypeModel;

#[derive(Serialize, Deserialize)]
pub struct Creation {
    pub name: String,
    pub quantity: i32,
    pub unit_id: i32,
    pub type_id: i32
}

#[derive(Serialize, Deserialize)]
pub struct CommonResponse {
    pub id: i32,
    pub name: String,
    pub quantity: i32,
    pub units: UnitModel,
    pub types: TypeModel
}

#[derive(Serialize, Deserialize)]
pub struct Update {
    pub id: i32,
    pub name: String,
    pub quantity: i32,
    pub unit_id: i32,
    pub type_id: i32
}

#[derive(Serialize, Deserialize)]
pub struct DeleteResponse {
    pub rows: u64
}