using Application;
using Application.BusinessLogic.FoodItemLogic;
using Application.BusinessLogic.MenuItemFoodItemLogic;
using Application.BusinessLogic.MenuItemLogic;
using Application.BusinessLogic.MenuLogic;
using Application.BusinessLogic.OrderItemLogic;
using Application.BusinessLogic.OrderLogic;
using Application.BusinessLogic.TypeLogic;
using Application.BusinessLogic.UnitLogic;
using Application.Dtos.FoodItem;
using Application.Dtos.Menu;
using Application.Dtos.MenuItem;
using Application.Dtos.MenuItemFoodItem;
using Application.Dtos.Order;
using Application.Dtos.OrderItem;
using Application.Dtos.Type;
using Application.Dtos.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RestaurantFoodPlanningSystem.Controllers;

public class DataManagementController(
    IUnit                             unit,
    IType                             type,
    IFoodItem                         foodItem,
    IMenu                             menu,
    IMenuItem                         menuItem,
    IMenuItemFoodItem                 menuItemFoodItem,
    IOrder                            order,
    IOrderItem                        orderItem,
    ILogger<DataManagementController> logger) : BaseApiController(logger)
{
    #region Unit

    /// <summary>
    /// Insert a record into table "Unit"
    /// </summary>
    /// <param name="queryDto">This object contains the name of the unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("unit/creation")]
    public async Task<ActionResult<Result<DbOperationResult<UnitResultDto>>>> CreateUnit(UnitQueryDto queryDto)
    {
        try
        {
            DbOperationResult<UnitResultDto> response = await unit.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UnitResultDto>>.Success(response));
            }

            logger.LogError($"Unit insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "Unit"
    /// </summary>
    /// <param name="queryDto">This object contains the name required to be updated and the id of a unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("unit/update")]
    public async Task<ActionResult<Result<DbOperationResult<UnitResultDto>>>> UpdateUnit(UnitQueryDto queryDto)
    {
        try
        {
            DbOperationResult<UnitResultDto> response = await unit.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UnitResultDto>>.Success(response));
            }

            logger.LogError($"Unit update failed: {response}");
            return HandlerResult(Result<string>.Failure("Update Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "Unit"
    /// </summary>
    /// <param name="queryDto">This object contains the name and id of a unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("unit/read")]
    public async Task<ActionResult<Result<DbOperationResult<UnitResultDto>>>> ReadUnit(UnitQueryDto queryDto)
    {
        try
        {
            DbOperationResult<UnitResultDto> response = await unit.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<UnitResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete specific record in table "Unit"
    /// </summary>
    /// <param name="id">This id of a unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("unit/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<UnitResultDto>>>> DeleteUnit(int id)
    {
        try
        {
            DbOperationResult<UnitResultDto> response = await unit.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UnitResultDto>>.Success(response));
            }

            logger.LogError($"Unit deletion failed: {response}");
            return HandlerResult(Result<string>.Failure("Deletion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region Type

    /// <summary>
    /// Insert a record into table "Type"
    /// </summary>
    /// <param name="queryDto">This object contains the name and id of a food category</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("type/creation")]
    public async Task<ActionResult<Result<DbOperationResult<TypeResultDto>>>> CreateType(TypeQueryDto queryDto)
    {
        try
        {
            DbOperationResult<TypeResultDto> response = await type.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<TypeResultDto>>.Success(response));
            }

            logger.LogError($"Type insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "Type"
    /// </summary>
    /// <param name="queryDto">This object contains the name and id of a food category</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("type/update")]
    public async Task<ActionResult<Result<DbOperationResult<TypeResultDto>>>> UpdateType(TypeQueryDto queryDto)
    {
        try
        {
            DbOperationResult<TypeResultDto> response = await type.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<TypeResultDto>>.Success(response));
            }

            logger.LogError($"Type update failed: {response}");
            return HandlerResult(Result<string>.Failure("Update Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "Type"
    /// </summary>
    /// <param name="queryDto">This object contains the name and id of a food category</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("type/read")]
    public async Task<ActionResult<Result<DbOperationResult<TypeResultDto>>>> ReadType(TypeQueryDto queryDto)
    {
        try
        {
            DbOperationResult<TypeResultDto> response = await type.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<TypeResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete record in table "Type"
    /// </summary>
    /// <param name="id">This object contains the id of a food category</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("type/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<TypeResultDto>>>> DeleteType(int id)
    {
        try
        {
            DbOperationResult<TypeResultDto> result = await type.Delete(id);

            if (result.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<TypeResultDto>>.Success(result));
            }

            logger.LogError($"Type deletion failed: {result}");
            return HandlerResult(Result<string>.Failure("Deletion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region FoodItem

    /// <summary>
    /// Insert a record into table "FoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains: id,  name, quantity, the id of type, and the id of unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("food-item/creation")]
    public async Task<ActionResult<Result<DbOperationResult<FoodItemResultDto>>>> CreateFoodItem(
        FoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<FoodItemResultDto> response = await foodItem.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<FoodItemResultDto>>.Success(response));
            }

            logger.LogError($"FoodItem insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "FoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains: id,  name, quantity, the id of type, and the id of unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("food-item/update")]
    public async Task<ActionResult<Result<DbOperationResult<FoodItemResultDto>>>> UpdateFoodItem(
        FoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<FoodItemResultDto> response = await foodItem.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<FoodItemResultDto>>.Success(response));
            }

            logger.LogError($"FoodItem update failed: {response}");
            return HandlerResult(Result<string>.Failure("Update Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "FoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains: id,  name, quantity, the id of type, and the id of unit</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("food-item/read")]
    public async Task<ActionResult<Result<DbOperationResult<FoodItemResultDto>>>> ReadFoodItem(
        FoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<FoodItemResultDto> response = await foodItem.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<FoodItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete record in table "FoodItem"
    /// </summary>
    /// <param name="id">the id of a food item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("food-item/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<FoodItemResultDto>>>> DeleteFoodItem(int id)
    {
        try
        {
            DbOperationResult<FoodItemResultDto> response = await foodItem.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<FoodItemResultDto>>.Success(response));
            }

            logger.LogError($"FoodItem update failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region MenuItem

    /// <summary>
    /// Insert a record into table "MenuItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id and name of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu-item/creation")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemResultDto>>>> CreateMenuItem(MenuItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemResultDto> response = await menuItem.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuItemResultDto>>.Success(response));
            }

            logger.LogError($"MenuItem insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "MenuItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id and name of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu-item/update")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemResultDto>>>> UpdateMenuItem(
        MenuItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemResultDto> response = await menuItem.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuItemResultDto>>.Success(response));
            }

            logger.LogError($"MenuItem update failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "MenuItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id and name of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("menu-item/read")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemResultDto>>>> ReadMenuItem(
        MenuItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemResultDto> response = await menuItem.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<MenuItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete a record in table "MenuItem"
    /// </summary>
    /// <param name="id">The id of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("menu-item/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemResultDto>>>> DeleteMenuItem(int id)
    {
        try
        {
            DbOperationResult<MenuItemResultDto> response = await menuItem.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuItemResultDto>>.Success(response));
            }

            logger.LogError($"MenuItem delete failed: {response}");
            return HandlerResult(Result<string>.Failure("Delete Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region Menu

    /// <summary>
    /// Insert a record into table "Menu"
    /// </summary>
    /// <param name="queryDto">This object contains: id, the date of a menu, the id of menu items</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu/creation")]
    public async Task<ActionResult<Result<DbOperationResult<MenuResultDto>>>> CreateMenu(MenuQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuResultDto> response = await menu.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuResultDto>>.Success(response));
            }

            logger.LogError($"Menu insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "Menu"
    /// </summary>
    /// <param name="queryDto">This object contains: id, the date of a menu, the id of menu items</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu/update")]
    public async Task<ActionResult<Result<DbOperationResult<MenuResultDto>>>> UpdateMenu(MenuQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuResultDto> response = await menu.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuResultDto>>.Success(response));
            }

            logger.LogError($"Menu update failed: {response}");
            return HandlerResult(Result<string>.Failure("Update Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "Menu"
    /// </summary>
    /// <param name="queryDto">This object contains: id, the date of a menu, the id of menu items</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("menu/read")]
    public async Task<ActionResult<Result<DbOperationResult<MenuResultDto>>>> ReadMenu(MenuQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuResultDto> response = await menu.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<MenuResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete a record from table "Menu"
    /// </summary>
    /// <param name="id">the id of a menu</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("menu/{id}")]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        try
        {
            DbOperationResult<MenuResultDto> response = await menu.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuResultDto>>.Success(response));
            }

            logger.LogError($"Menu delete failed: {response}");
            return HandlerResult(Result<string>.Failure("Delete failed."));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region MenuItemFoodItem

    /// <summary>
    /// Insert a record into table "MenuItemFoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of a menu item, the id of a food item, and the consumption</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu-item-food-item/creation")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemFoodItemResultDto>>>> CreateMenuItemFoodItem(
        MenuItemFoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemFoodItemResultDto> response = await menuItemFoodItem.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuItemFoodItemResultDto>>.Success(response));
            }

            logger.LogError($"MenuItem insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "MenuItemFoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of a menu item, the id of a food item, and the consumption</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("menu-item-food-item/update")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemFoodItemResultDto>>>> UpdateMenuItemFoodItem(
        MenuItemFoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemFoodItemResultDto> response = await menuItemFoodItem.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<MenuItemFoodItemResultDto>>.Success(response));
            }

            logger.LogError($"MenuItem insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "MenuItemFoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of a menu item, the id of a food item, and the consumption</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("menu-item-food-item/read")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemFoodItemResultDto>>>> ReadMenuItemFoodItem(
        MenuItemFoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemFoodItemResultDto> response = await menuItemFoodItem.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<MenuItemFoodItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete record in table "MenuItemFoodItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of a menu item, the id of a food item, and the consumption</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("menu-item-food-item")]
    public async Task<ActionResult<Result<DbOperationResult<MenuItemFoodItemResultDto>>>> DeleteMenuItemFoodItem(
        MenuItemFoodItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<MenuItemFoodItemResultDto> response = await menuItemFoodItem.Delete(queryDto);

            return HandlerResult(Result<DbOperationResult<MenuItemFoodItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region Order

    /// <summary>
    /// Insert record into table "Order"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order and the flag of cancellation</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("order/creation")]
    public async Task<ActionResult<Result<DbOperationResult<OrderResultDto>>>> CreateOrder(OrderQueryDto queryDto)
    {
        try
        {
            DbOperationResult<OrderResultDto> response = await order.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<OrderResultDto>>.Success(response));
            }

            logger.LogError($"Order insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update record in table "Order"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order and the flag of cancellation</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("order/update")]
    public async Task<ActionResult<Result<DbOperationResult<OrderResultDto>>>> UpdateOrder(OrderQueryDto queryDto)
    {
        try
        {
            DbOperationResult<OrderResultDto> response = await order.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<OrderResultDto>>.Success(response));
            }

            logger.LogError($"Order insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "Order"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order and the flag of cancellation</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("order/read")]
    public async Task<ActionResult<Result<DbOperationResult<OrderResultDto>>>> ReadOrder(OrderQueryPerPageDto queryDto)
    {
        try
        {
            DbOperationResult<OrderResultDto> response = await order.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<OrderResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete a record from table "Order"
    /// </summary>
    /// <param name="id">The id of an order</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("order/{id}")]
    public async Task<ActionResult<Result<DbOperationResult<OrderResultDto>>>> DeleteOrder(int id)
    {
        try
        {
            DbOperationResult<OrderResultDto> result = await order.Delete(id);

            return HandlerResult(Result<DbOperationResult<OrderResultDto>>.Success(result));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion

    #region OrderItem

    /// <summary>
    /// Insert a record into table "OrderItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order item, the id of an order, and the id of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("order-item/creation")]
    public async Task<ActionResult<DbOperationResult<OrderItemResultDto>>> CreateOrder(OrderItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<OrderItemResultDto> response = await orderItem.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<OrderItemResultDto>>.Success(response));
            }

            logger.LogError($"OrderItem insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "OrderItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order item, the id of an order, and the id of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("order-item/update")]
    public async Task<ActionResult<DbOperationResult<OrderItemResultDto>>> UpdateOrder(OrderItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<OrderItemResultDto> response = await orderItem.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<OrderItemResultDto>>.Success(response));
            }

            logger.LogError($"OrderItem update failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Retrieve records from table "OrderItem"
    /// </summary>
    /// <param name="queryDto">This object contains the id of an order item, the id of an order, and the id of a menu item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "StaffAndManager")]
    [HttpPost("order-item/read")]
    public async Task<ActionResult<DbOperationResult<OrderItemResultDto>>> ReadOrder(OrderItemQueryDto queryDto)
    {
        try
        {
            DbOperationResult<OrderItemResultDto> response = await orderItem.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<OrderItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete a record in table "OrderItem"
    /// </summary>
    /// <param name="id">The id of an order item</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("order-item/{id}")]
    public async Task<ActionResult<DbOperationResult<OrderItemResultDto>>> DeleteOrderItem(int id)
    {
        try
        {
            DbOperationResult<OrderItemResultDto> response = await orderItem.Delete(id);

            return HandlerResult(Result<DbOperationResult<OrderItemResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    #endregion
}