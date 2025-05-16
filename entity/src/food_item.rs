use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "food_item")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: i32,
    pub name: String,
    pub quantity: i32,
    pub unit_id: i32,
    pub type_id: i32,
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
            Self::MenuItemFoodItem => Entity::has_many(super::menu_item_food_item::Entity).into(),
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
