use sea_orm::{ConnectOptions, Database, DatabaseConnection};
use std::borrow::Cow;

#[derive(Clone)]
pub struct EnvironmentVariable {
    pub database_connection: DatabaseConnection,
}

impl EnvironmentVariable {
    pub async fn from_env() -> anyhow::Result<Self> {
        let mut opt = ConnectOptions::new(
            std::env::var("DATABASE_URL")
                .map(Cow::from)
                .expect("DATABASE_URL must be set"),
        );
        if is_dev() {
            opt.sqlx_logging_level(log::LevelFilter::Trace);
        }
        Ok(Self {
            database_connection: Database::connect(opt).await?,
        })
    }
}

fn is_dev() -> bool {
    match std::env::var("RUST_ENV") {
        Ok(val) => val == "development",
        Err(_) => false,
    }
}
