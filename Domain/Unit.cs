namespace Domain;

public class Unit
{
    public int    Id   { get; set; }
    public String Name { get; set; }
    
    public ICollection<FoodItem> FoodItems { get; set; }
}