using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class MenuItemFoodItem
{
    public int MenuItem_Id { get; set; }
    
    public int      FoodItem_Id { get; set; }
    
    public int      Consumption { get; set; }
    
    public FoodItem? FoodItem { get; set; }
    public MenuItem? MenuItem { get; set; }
}