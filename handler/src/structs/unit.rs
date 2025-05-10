use serde::{Deserialize};

#[derive(Deserialize)]
pub struct Creation {
    pub id: i32,
    pub name: String,
}