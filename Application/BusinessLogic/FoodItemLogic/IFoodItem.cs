using Application.Dtos.FoodItem;
using Domain;

namespace Application.BusinessLogic.FoodItemLogic
{
    public interface IFoodItem
    {
        public Task<DbOperationResult<FoodItemResultDto>> Insert(FoodItemQueryDto foodItemQuery);

        public Task<DbOperationResult<FoodItemResultDto>> Update(FoodItemQueryDto foodItemQuery);

        public Task<DbOperationResult<FoodItemResultDto>> Read(FoodItemQueryDto foodItemQuery);

        public Task<DbOperationResult<FoodItemResultDto>> Delete(int id);
    }
}