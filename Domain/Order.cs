using System.Text.Json.Serialization;

namespace Domain;

public class Order
{
    public int                     Id         { get; set; }
    public bool                    IsCanceled { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}