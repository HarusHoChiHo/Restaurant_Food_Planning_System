using Application.Dtos.FoodItem;
using Application.Dtos.Order;
using Domain;

namespace Application.BusinessLogic.OrderLogic
{
    public interface IOrder
    {
        public Task<DbOperationResult<OrderResultDto>> Insert(OrderQueryDto orderQuery);

        public Task<DbOperationResult<OrderResultDto>> Update(OrderQueryDto orderQuery);

        public Task<DbOperationResult<OrderResultDto>> Read(OrderQueryPerPageDto orderQuery);

        public Task<DbOperationResult<OrderResultDto>> Delete(int id);
    }
}