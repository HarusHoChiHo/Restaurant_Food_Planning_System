using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class MenuItem
{
    public int Id { get; set; }

    public String Name { get; set; }

    public ICollection<MenuItemFoodItem> MenuItemFoodItems { get; set; }
    public ICollection<Menu>             Menus             { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}