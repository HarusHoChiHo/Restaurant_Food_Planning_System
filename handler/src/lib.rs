use crate::req_res_structs::env::EnvironmentVariable;

pub mod data_management;
mod req_res_structs;


#[derive(Clone)]
pub struct AppState {
    pub env: EnvironmentVariable,
}

pub async fn init_db_con() -> AppState {
    let env = EnvironmentVariable::from_env().await.unwrap();
    
    AppState { env }
}