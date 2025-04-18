using Application;
using Application.BusinessLogic.FoodItemLogic;
using Application.BusinessLogic.MenuItemFoodItemLogic;
using Application.BusinessLogic.OrderItemLogic;
using Application.BusinessLogic.OrderLogic;
using Application.Dtos.FoodItem;
using Application.Dtos.MenuItemFoodItem;
using Application.Dtos.Order;
using Application.Dtos.OrderHandling;
using Application.Dtos.OrderItem;
using Application.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RestaurantFoodPlanningSystem.Controllers;

public class OrderController(
    IOrder                   order,
    IOrderItem               orderItem,
    IMenuItemFoodItem        menuItemFoodItem,
    IFoodItem                foodItem,
    ILogger<OrderController> logger) : BaseApiController(logger)
{
    /// <summary>
    /// Place an order
    /// </summary>
    /// <param name="queryDto">This object contains the object "OrderQueryDto" and a list of "OrderItemQueryDto"</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("place-order")]
    public async Task<ActionResult<Result<OrderPlacementResDto>>> PlaceOrder(OrderPlacementQueryDto queryDto)
    {
        try
        {
            List<MenuItemFoodItemResultDto> mifiDto = new List<MenuItemFoodItemResultDto>();
            List<FoodItemResultDto>         fiDto   = new List<FoodItemResultDto>();

            queryDto.orderItems.ForEach(
                                        orderItem =>
                                        {
                                            var tempMifi = menuItemFoodItem.Read(
                                                                                 new MenuItemFoodItemQueryDto()
                                                                                 {
                                                                                     MenuItem_Id = orderItem.MenuItemId
                                                                                 })
                                                                           .Result.resultDto;

                                            if (tempMifi != null && tempMifi.Count > 0)
                                            {
                                                mifiDto.AddRange(tempMifi);
                                            }
                                        });

            List<int> groupedFoodItemId = mifiDto
                                          .GroupBy(fi => fi.FoodItem_Id)
                                          .Select(dto => dto.Key)
                                          .ToList();

            groupedFoodItemId.ForEach(
                                      id =>
                                      {
                                          var fiTemp = foodItem.Read(
                                                                     new FoodItemQueryDto()
                                                                     {
                                                                         Id = id
                                                                     })
                                                               .Result.resultDto;

                                          if (fiTemp != null)
                                          {
                                              fiDto.AddRange(fiTemp);
                                          }
                                      });

            var groupedConsumption = mifiDto
                                     .GroupBy(
                                              item =>
                                                  new
                                                  {
                                                      item.FoodItem_Id
                                                  })
                                     .Select(
                                             item =>
                                                 new
                                                 {
                                                     item.Key.FoodItem_Id,
                                                     TotalConsumption = item.Sum(x => x.Consumption)
                                                 })
                                     .ToList();

            if (!groupedConsumption.TrueForAll(
                                               item =>
                                               {
                                                   var temp = fiDto.Find(x => x.Id == item.FoodItem_Id);
                                                   return item.TotalConsumption <= temp.Quantity;
                                               }))
            {
                return HandlerResult(Result<string>.Failure("Some food is sold out."));
            }

            OrderPlacementResDto response = new OrderPlacementResDto();

            var orderInsertion = await order.Insert(queryDto.order);

            /*response.orderItemResDtos = queryDto.orderItems.ConvertAll(
                                                                       item =>
                                                                       {
                                                                           item.OrderId = orderInsertion
                                                                                          .resultDto.First()
                                                                                          .Id;
                                                                           var oi = orderItem.Insert(item)
                                                                                           .Result.resultDto.First();

                                                                           return orderItem
                                                                                  .Read(
                                                                                        new OrderItemQueryDto()
                                                                                        {
                                                                                            Id = oi.Id
                                                                                        })
                                                                                  .Result;
                                                                       });*/

            queryDto.orderItems.ForEach(
                                        item =>
                                        {
                                            if (orderInsertion
                                                    .resultDto != null)
                                            {
                                                item.OrderId = orderInsertion
                                                               .resultDto.First()
                                                               .Id;
                                                var oi = orderItem.Insert(item)
                                                                  .Result;
                                            }
                                        });

            response.orderResDto = await order.Read(
                                                    new OrderQueryPerPageDto()
                                                    {
                                                        Id = orderInsertion.resultDto.First()
                                                                           .Id
                                                    });

            groupedConsumption.ForEach(
                                       x =>
                                       {
                                           FoodItemResultDto temp = fiDto.Find(dto => dto.Id == x.FoodItem_Id);
                                           int               restQuantity;
                                           if (temp != null)
                                           {
                                               restQuantity = temp.Quantity - x.TotalConsumption;

                                               var updateResult = foodItem.Update(
                                                                                  new FoodItemQueryDto()
                                                                                  {
                                                                                      Id       = x.FoodItem_Id,
                                                                                      Quantity = restQuantity
                                                                                  })
                                                                          .Result;

                                               if (updateResult.amount == 0)
                                               {
                                                   logger.LogError(JsonConvert.SerializeObject(updateResult));
                                               }
                                           }
                                           else
                                           {
                                               logger.LogError(
                                                               $"Failed to find object with FoodItem_Id: {x.FoodItem_Id}");
                                           }
                                       });

            return HandlerResult(Result<OrderPlacementResDto>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    /// <param name="id">This is the id of a specific order</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpGet("cancel-order/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<OrderResultDto>>>> CancelOrder(int id)
    {
        try
        {
            OrderQueryDto dto = new OrderQueryDto()
                                {
                                    Id         = id,
                                    IsCanceled = true
                                };

            DbOperationResult<OrderResultDto> response = await order.Update(dto);

            return HandlerResult(Result<DbOperationResult<OrderResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }
}