using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Type
{
    public int    Id   { get; set; }
    public String Name { get; set; }
    
    public ICollection<FoodItem> FoodItems { get; set; }
}