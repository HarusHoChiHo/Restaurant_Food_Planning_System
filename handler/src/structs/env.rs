use std::borrow::Cow;

#[derive(Clone)]
pub struct EnvironmentVariable {
    pub database_url: Cow<'static, str>,
}

impl EnvironmentVariable {
    pub fn from_env() -> anyhow::Result<Self> {
        Ok(Self {
            database_url: std::env::var("DATABASE_URL")
                .map(Cow::from)
                .expect("DATABASE_URL must be set"),
        })
    }
}