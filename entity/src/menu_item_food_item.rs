use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "menu_item_food_item")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub menu_item_id: i32,
    #[sea_orm(primary_key)]
    pub food_item_id: i32,
    pub consumption: f32,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    FoodItem,
    MenuItem,
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::MenuItem => Entity::belongs_to(super::menu_item::Entity)
                .from(Column::MenuItemId)
                .to(super::menu_item::Column::Id)
                .into(),
            Self::FoodItem => Entity::belongs_to(super::food_item::Entity)
                .from(Column::FoodItemId)
                .to(super::food_item::Column::Id)
                .into(),
        }
    }
}

impl Related<super::menu_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::MenuItem.def()
    }
}

impl Related<super::food_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::FoodItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}
