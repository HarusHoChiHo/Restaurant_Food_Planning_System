use sea_orm_migration::{prelude::*, schema::*};

#[derive(DeriveMigrationName)]
pub struct Migration;

#[async_trait::async_trait]
impl MigrationTrait for Migration {
    async fn up(&self, manager: &SchemaManager) -> Result<(), DbErr> {
        // Replace the sample below with your own migration scripts
        manager
            .create_table(
                Table::create()
                    .table(MenuItem::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(MenuItem::Id)
                            .integer()
                            .auto_increment()
                            .primary_key()
                            .not_null(),
                    )
                    .col(ColumnDef::new(MenuItem::Name).string().not_null())
                    .to_owned(),
            )
            .await?;

        manager
            .create_table(
                Table::create()
                    .table(Unit::Table)
                    .if_not_exists()
                    .col(pk_auto(Unit::Id))
                    .col(string(Unit::Name))
                    .to_owned(),
            )
            .await
            .expect("Create Unit table failed");

        manager
            .create_table(
                Table::create()
                    .table(Type::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(Type::Id)
                            .integer()
                            .not_null()
                            .primary_key()
                            .auto_increment(),
                    )
                    .col(ColumnDef::new(Type::Name).string().not_null())
                    .to_owned(),
            )
            .await?;

        manager
            .create_table(
                Table::create()
                    .table(FoodItem::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(FoodItem::Id)
                            .integer()
                            .not_null()
                            .primary_key()
                            .auto_increment(),
                    )
                    .col(ColumnDef::new(FoodItem::Name).string().not_null())
                    .col(ColumnDef::new(FoodItem::TypeId).integer().not_null())
                    .col(ColumnDef::new(FoodItem::UnitId).integer().not_null())
                    .col(
                        ColumnDef::new(FoodItem::Quantity)
                            .integer()
                            .not_null()
                            .default(0),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-food-item-type-id")
                            .from(FoodItem::Table, FoodItem::TypeId)
                            .to(Type::Table, Type::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-food-item-unit-id")
                            .from(FoodItem::Table, FoodItem::UnitId)
                            .to(Unit::Table, Unit::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .to_owned(),
            )
            .await?;

        manager
            .create_table(
                Table::create()
                    .table(Order::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(Order::Id)
                            .integer()
                            .not_null()
                            .auto_increment()
                            .primary_key(),
                    )
                    .col(ColumnDef::new(Order::IsCanceled).boolean().not_null())
                    .col(ColumnDef::new(Order::OrderDate).date_time().not_null())
                    .to_owned(),
            )
            .await?;

        manager
            .create_table(
                Table::create()
                    .table(OrderItem::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(OrderItem::Id)
                            .integer()
                            .auto_increment()
                            .primary_key()
                            .not_null(),
                    )
                    .col(ColumnDef::new(OrderItem::OrderId).integer().not_null())
                    .col(ColumnDef::new(OrderItem::MenuItemId).integer().not_null())
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-menu-item-id")
                            .from(OrderItem::Table, OrderItem::MenuItemId)
                            .to(MenuItem::Table, MenuItem::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-order-id")
                            .from(OrderItem::Table, OrderItem::OrderId)
                            .to(Order::Table, Order::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .to_owned(),
            )
            .await?;

        manager
            .create_table(
                Table::create()
                    .table(MenuItemFoodItem::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(MenuItemFoodItem::Consumption)
                            .integer()
                            .not_null()
                    )
                    .col(
                        ColumnDef::new(MenuItemFoodItem::MenuItemId)
                            .integer()
                            .not_null(),
                    )
                    .col(
                        ColumnDef::new(MenuItemFoodItem::FoodItemId)
                            .integer()
                            .not_null(),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-menu-item-food-item-menu-item-id")
                            .from(MenuItemFoodItem::Table, MenuItemFoodItem::MenuItemId)
                            .to(MenuItem::Table, MenuItem::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-menu-item-food-item-food-item-id")
                            .from(MenuItemFoodItem::Table, MenuItemFoodItem::FoodItemId)
                            .to(FoodItem::Table, FoodItem::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .to_owned(),
            )
            .await?;



        manager
            .create_table(
                Table::create()
                    .table(Menu::Table)
                    .if_not_exists()
                    .col(
                        ColumnDef::new(Menu::Id)
                            .integer()
                            .primary_key()
                            .not_null()
                            .auto_increment(),
                    )
                    .col(ColumnDef::new(Menu::MenuItemId).integer().not_null())
                    .col(
                        ColumnDef::new(Menu::Date)
                            .date()
                            .not_null()
                            .default(Expr::current_date()),
                    )
                    .foreign_key(
                        ForeignKey::create()
                            .name("fk-menu-menu-item-id")
                            .from(Menu::Table, Menu::MenuItemId)
                            .to(MenuItem::Table, MenuItem::Id)
                            .on_delete(ForeignKeyAction::Cascade)
                            .on_update(ForeignKeyAction::Cascade),
                    )
                    .to_owned(),
            )
            .await?;


        Ok(())
    }

    async fn down(&self, manager: &SchemaManager) -> Result<(), DbErr> {
        // Replace the sample below with your own migration scripts

        manager
            .drop_table(Table::drop().table(Unit::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(Type::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(OrderItem::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(Order::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(MenuItemFoodItem::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(Menu::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(MenuItem::Table).to_owned())
            .await?;

        manager
            .drop_table(Table::drop().table(FoodItem::Table).to_owned())
            .await?;

        Ok(())
    }
}

#[derive(DeriveIden)]
enum Type {
    Table,
    Id,
    Name,
}

#[derive(DeriveIden)]
enum Unit {
    Table,
    Id,
    Name,
}

#[derive(DeriveIden)]
enum Order {
    Table,
    Id,
    IsCanceled,
    OrderDate,
}

#[derive(DeriveIden)]
enum OrderItem {
    Table,
    Id,
    OrderId,
    MenuItemId,
}

#[derive(DeriveIden)]
enum MenuItemFoodItem {
    Table,
    MenuItemId,
    FoodItemId,
    Consumption,
}

#[derive(DeriveIden)]
enum MenuItem {
    Table,
    Id,
    Name,
}

#[derive(DeriveIden)]
enum Menu {
    Table,
    Id,
    Date,
    MenuItemId,
}

#[derive(DeriveIden)]
enum FoodItem {
    Table,
    Id,
    Name,
    Quantity,
    UnitId,
    TypeId,
}
