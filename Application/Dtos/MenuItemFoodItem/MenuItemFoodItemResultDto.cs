using Application.Dtos.FoodItem;
using Application.Dtos.MenuItem;

namespace Application.Dtos.MenuItemFoodItem
{
    public class MenuItemFoodItemResultDto
    {
        public int MenuItem_Id { get; set; }
        public int FoodItem_Id { get; set; }
        public int Consumption { get; set; }

        public MenuItemResultDto MenuItem { get; set; }
        
        public FoodItemResultDto FoodItem { get; set; }
    }
}