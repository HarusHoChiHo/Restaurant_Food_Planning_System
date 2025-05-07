use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "food_items")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: u32,
    pub name: String,
    pub quantity: u32,
    pub unit_id: u32,
    pub type_id: u32,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    Units,
    Types,
    MenuItemFoodItem,
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::Units => Entity::has_many(super::units::Entity).into(),
            Self::Types => Entity::has_many(super::types::Entity).into(),
            Self::MenuItemFoodItem => Entity::belongs_to(super::menu_item_food_item::Entity)
                .from(Column::Id)
                .to(super::menu_item_food_item::Column::FoodItemId)
                .into(),
        }
    }
}

impl Related<super::units::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::Units.def()
    }
}

impl Related<super::types::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::Types.def()
    }
}

impl Related<super::menu_item_food_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::MenuItemFoodItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}
