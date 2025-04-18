using Application.Dtos.Order;
using Application.Dtos.OrderItem;

namespace Application.Dtos.OrderHandling
{
    public class OrderPlacementQueryDto
    {
        public OrderQueryDto           order      { get; set; }
        public List<OrderItemQueryDto> orderItems { get; set; }
    }
}