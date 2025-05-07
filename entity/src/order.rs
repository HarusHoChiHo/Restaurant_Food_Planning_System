use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "orders")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: i32,
    pub is_canceled: bool,
    pub order_date: DateTime,
}

#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    OrderItem,
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::OrderItem => Entity::belongs_to(super::order_item::Entity)
                .from(Column::Id)
                .to(super::order_item::Column::OrderId)
                .into(),
        }
    }
}

impl Related<super::order_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::OrderItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}
