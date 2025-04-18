namespace Domain;

public class OrderItem
{
    public int Id         { get; set; }
    public int OrderId    { get; set; }
    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }
    public Order    Order    { get; set; }
}