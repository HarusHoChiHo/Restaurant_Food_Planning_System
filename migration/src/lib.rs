pub use sea_orm_migration::prelude::*;
mod m20250507_205549_create_table;
mod m20250517_152401_update_menu_item_food_item;
mod m20250517_174601_add_primary_key_to_mifi;

pub struct Migrator;

#[async_trait::async_trait]
impl MigratorTrait for Migrator {
    fn migrations() -> Vec<Box<dyn MigrationTrait>> {
        vec![
            Box::new(m20250507_205549_create_table::Migration),
            Box::new(m20250517_152401_update_menu_item_food_item::Migration),
            Box::new(m20250517_174601_add_primary_key_to_mifi::Migration),
        ]
    }
}
