using Application.Dtos.MenuItemFoodItem;

namespace Application.BusinessLogic.MenuItemFoodItemLogic
{
    public interface IMenuItemFoodItem
    {
        public Task<DbOperationResult<MenuItemFoodItemResultDto>> Insert(MenuItemFoodItemQueryDto menuIteFoodItemQuery);

        public Task<DbOperationResult<MenuItemFoodItemResultDto>> Update(MenuItemFoodItemQueryDto menuItemFoodItemQuery);

        public Task<DbOperationResult<MenuItemFoodItemResultDto>> Read(
            MenuItemFoodItemQueryDto menuItemFoodItemQueryDto);

        public Task<DbOperationResult<MenuItemFoodItemResultDto>> Delete(MenuItemFoodItemQueryDto queryDto);
    }
}