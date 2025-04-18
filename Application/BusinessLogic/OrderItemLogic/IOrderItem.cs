using Application.Dtos.OrderItem;

namespace Application.BusinessLogic.OrderItemLogic
{
    public interface IOrderItem
    {
        public Task<DbOperationResult<OrderItemResultDto>> Insert(OrderItemQueryDto orderItemQuery);

        public Task<DbOperationResult<OrderItemResultDto>> Update(OrderItemQueryDto orderItemQuery);

        public Task<DbOperationResult<OrderItemResultDto>> Read(OrderItemQueryDto orderItemQuery);

        public Task<DbOperationResult<OrderItemResultDto>> Delete(int id);
    }
}