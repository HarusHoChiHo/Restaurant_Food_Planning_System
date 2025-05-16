use sea_orm::{Database, DatabaseConnection};
use std::borrow::Cow;

#[derive(Clone)]
pub struct EnvironmentVariable {
    pub database_connection: DatabaseConnection,
}

impl EnvironmentVariable {
    pub async fn from_env() -> anyhow::Result<Self> {
        Ok(Self {
            database_connection: Database::connect(
                std::env::var("DATABASE_URL")
                    .map(Cow::from)
                    .expect("DATABASE_URL must be set"),
            )
            .await?,
        })
    }
}
