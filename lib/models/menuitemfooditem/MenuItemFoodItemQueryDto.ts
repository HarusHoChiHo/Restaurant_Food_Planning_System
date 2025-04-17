interface IMenuItemFoodItemQueryDto {
    menuItem_Id: number | null;
    foodItem_Id: number | null;
    consumption: number | null;
}

class MenuItemFoodItemQueryDto implements IMenuItemFoodItemQueryDto {
    menuItem_Id: number | null;
    foodItem_Id: number | null;
    consumption: number | null;

    constructor(menuItem_Id: number | null, foodItem_Id: number | null, consumption: number | null) {
        this.menuItem_Id = menuItem_Id;
        this.foodItem_Id = foodItem_Id;
        this.consumption = consumption;
    }
}

export default MenuItemFoodItemQueryDto;