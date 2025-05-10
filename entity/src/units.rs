use sea_orm::entity::prelude::*;
use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Eq, DeriveEntityModel, Serialize, Deserialize)]
#[sea_orm(table_name = "unit")]
pub struct Model {
    #[sea_orm(primary_key)]
    pub id: i32,
    pub name: String,
}
#[derive(Copy, Clone, Debug, EnumIter)]
pub enum Relation {
    FoodItem,
}

impl RelationTrait for Relation {
    fn def(&self) -> RelationDef {
        match self {
            Self::FoodItem => Entity::belongs_to(super::food_item::Entity)
                .from(Column::Id)
                .to(super::food_item::Column::UnitId)
                .into(),
        }
    }
}

impl Related<super::food_item::Entity> for Entity {
    fn to() -> RelationDef {
        Relation::FoodItem.def()
    }
}

impl ActiveModelBehavior for ActiveModel {}
