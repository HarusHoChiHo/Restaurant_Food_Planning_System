using Application.Dtos.MenuItem;

namespace Application.Dtos.OrderItem
{
    public class OrderItemResultDto
    {
        public int               Id       { get; set; }
        public int               OrderId  { get; set; }
        public MenuItemResultDto MenuItem { get; set; }
    }
}