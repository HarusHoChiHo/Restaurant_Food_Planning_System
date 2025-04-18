using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class FoodItem
{
    public int    Id       { get; set; }
    
    public String Name     { get; set; }
    
    public int    Quantity { get; set; }
    
    public int    Unit_Id  { get; set; }
    
    public int    Type_Id  { get; set; }
    
    
    public Type                          Type              { get; set; }
    public Unit                          Unit              { get; set; }
    public ICollection<MenuItemFoodItem> MenuItemFoodItems { get; set; }
}