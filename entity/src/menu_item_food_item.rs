use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "menu_item_food_item")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub menu_item_id: u32,
    #[sea_orm(primary_key)]
    pub food_item_id: u32,
    pub consumption: f32,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    FoodItem,
    MenuItem
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::MenuItem => Entity::has_many(super::menu_item::Entity).into(),
            Self::FoodItem => Entity::has_many(super::food_item::Entity).into()
        }
    }
}

impl Related<super::menu_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::FoodItem.def()
    }
}

impl Related<super::food_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::FoodItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}