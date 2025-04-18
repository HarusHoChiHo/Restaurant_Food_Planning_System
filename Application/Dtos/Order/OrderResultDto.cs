using Application.Dtos.OrderItem;

namespace Application.Dtos.Order
{
    public class OrderResultDto
    {
        public int                       Id         { get; set; }
        public bool                      IsCanceled { get; set; }
        public IList<OrderItemResultDto> OrderItems { get; set; }
    }
}