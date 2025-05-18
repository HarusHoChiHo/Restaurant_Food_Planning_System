use sea_orm_migration::sea_orm::{Statement};
use sea_orm_migration::{prelude::*};

#[derive(DeriveMigrationName)]
pub struct Migration;

#[async_trait::async_trait]
impl MigrationTrait for Migration {
    async fn up(&self, manager: &SchemaManager) -> Result<(), DbErr> {
        let stmt = Statement::from_string(
            manager.get_database_backend(),
            r#"
                ALTER TABLE menu_item_food_item
                ADD CONSTRAINT menu_item_food_item_pk PRIMARY KEY (menu_item_id, food_item_id);
                "#,
        );

        manager.get_connection().execute(stmt).await?;

        Ok(())
    }

    async fn down(&self, manager: &SchemaManager) -> Result<(), DbErr> {
        let stmt = Statement::from_string(manager.get_database_backend(), r#"
                ALTER TABLE menu_item_food_item
                DROP CONSTRAINT menu_item_food_item_pk;
                "#);
        
        manager.get_connection().execute(stmt).await?;
        
        Ok(())
    }
}
