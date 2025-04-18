using Application;
using Application.BusinessLogic.RoleLogic;
using Application.Dtos.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RestaurantFoodPlanningSystem.Controllers;

public class RoleController(
    IRole                   role,
    ILogger<UserController> logger) : BaseApiController(logger)
{
    
    /// <summary>
    /// Insert a record into table "AspNetRoles"
    /// </summary>
    /// <param name="basicDto">This object contains the name of a role</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("creation")]
    public async Task<ActionResult<Result<DbOperationResult<RoleResultDto>>>> CreateRole(RoleBasicDto basicDto)
    {
        try
        {
            DbOperationResult<RoleResultDto> response = await role.Insert(basicDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<RoleResultDto>>.Success(response));
            }

            logger.LogError($"Role insertion failed: {response}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Update a record in table "AspNetRoles"
    /// </summary>
    /// <param name="fullDto">This object contains the name required to be updated and the id of a role</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("update")]
    public async Task<ActionResult<Result<DbOperationResult<RoleResultDto>>>> UpdateRole(RoleFullDto fullDto)
    {
        try
        {
            DbOperationResult<RoleResultDto> response = await role.Update(fullDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<RoleResultDto>>.Success(response));
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
    /// Retrieve records from table "AspNetRoles"
    /// </summary>
    /// <param name="queryDto">This object contains the name and id of a role</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpPost("read")]
    public async Task<ActionResult<Result<DbOperationResult<List<RoleResultDto>>>>> ReadRole(RoleQueryDto queryDto)
    {
        try
        {
            DbOperationResult<RoleResultDto> response = await role.Read(queryDto);

            return HandlerResult(Result<DbOperationResult<RoleResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }
    
    /// <summary>
    /// Delete specific record in table "AspNetRoles"
    /// </summary>
    /// <param name="id">This id of a role</param>
    /// <returns name="ActionResult">Http Response with object "Result"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<DbOperationResult<RoleResultDto>>>> DeleteRole(int id)
    {
        try
        {
            DbOperationResult<RoleResultDto> response = await role.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<RoleResultDto>>.Success(response));
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
}