use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "order_item")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: i32,
    pub order_id: i32,
    pub menu_item_id: i32,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    MenuItem,
    Order
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self { 
            Self::Order => Entity::has_many(super::order::Entity).into(),
            Self::MenuItem => Entity::has_many(super::menu_item::Entity).into()
        }
    }
}

impl Related<super::menu_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::MenuItem.def()
    }
}

impl Related<super::order::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::Order.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}



