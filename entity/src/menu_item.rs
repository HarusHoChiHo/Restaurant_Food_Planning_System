use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "menu_item")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: i32,
    pub name: String,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    MenuItemFoodItem,
    Menu,
    OrderItem,
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::MenuItemFoodItem => Entity::belongs_to(super::menu_item_food_item::Entity)
                .from(Column::Id)
                .to(super::menu_item_food_item::Column::MenuItemId)
                .into(),
            Self::Menu => Entity::belongs_to(super::menu::Entity)
                .from(Column::Id)
                .to(super::menu::Column::MenuItemId)
                .into(),
            Self::OrderItem => Entity::belongs_to(super::order_item::Entity)
                .from(Column::Id)
                .to(super::order_item::Column::MenuItemId)
                .into(),
        }
    }
}

impl Related<super::menu_item_food_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::MenuItemFoodItem.def()
    }
}

impl Related<super::menu::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::Menu.def()
    }
}

impl Related<super::order_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::OrderItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}
