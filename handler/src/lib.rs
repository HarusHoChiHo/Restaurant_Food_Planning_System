use sea_orm::{Iden};
use crate::structs::env::EnvironmentVariable;

pub mod data_management;
mod structs;


#[derive(Clone)]
pub struct AppState {
    pub env: EnvironmentVariable,
}
