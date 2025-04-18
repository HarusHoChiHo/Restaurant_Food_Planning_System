using Application;
using Application.BusinessLogic.RoleLogic;
using Application.BusinessLogic.UserLogic;
using Application.Dtos.Role;
using Application.Dtos.User;
using Application.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantFoodPlanningSystem.Services;

namespace RestaurantFoodPlanningSystem.Controllers;

public class UserController(
    IUser                   user,
    IRole                   role,
    TokenService            tokenService,
    ILogger<UserController> logger) : BaseApiController(logger)
{
    /// <summary>
    /// Send login information to validate
    /// </summary>
    /// <param name="basicDto">The object containing username and password.</param>
    /// <returns name="ActionResult">Http Response with object "UserResultDto"</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<Result<UserResDto<UserResultDto>>>> Login(UserBasicDto basicDto)
    {
        try
        {
            UserResDto<UserResultDto> response = new UserResDto<UserResultDto>();

            response.resultDto = new List<UserResultDto>()
                                 {
                                     await user.Validate(basicDto)
                                 };

            if (response.resultDto != null)
            {
                response.Token = tokenService.CreateToken(response.resultDto.First());
                IdentityResult identityResult = await user.SaveToken(
                                                                     response.resultDto.First().Id,
                                                                     "Local",
                                                                     "AccessToken",
                                                                     response.Token);

                if (!identityResult.Succeeded)
                {
                    logger.LogError("Failed to save token.");
                    return HandlerResult(Result<string>.Failure(JsonConvert.SerializeObject(identityResult.Errors)));
                }

                return HandlerResult(Result<UserResDto<UserResultDto>>.Success(response));
            }

            return HandlerResult(Result<UserResDto<UserResultDto>>.Failure("Invalid Login Info"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Send user information to do registration
    /// </summary>
    /// <param name="queryDto">The object containing username and password.</param>
    /// <returns name="ActionResult">Http Response with object "UserResultDto"</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<Result<DbOperationResult<UserResultDto>>>> Register(UserQueryDto queryDto)
    {
        try
        {
            DbOperationResult<UserResultDto> response = await user.Insert(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogError($"Insertion Failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Insertion Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Get a specific user information by user's id
    /// </summary>
    /// <param name="id">The integer id of a user</param>
    /// <returns name="ActionResult">Http Response with object "UserResultDto"</returns>
    [Authorize(Policy = "ManagerOnly")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<DbOperationResult<UserResultDto>>>> GetCurrentUser(int id)
    {
        try
        {
            DbOperationResult<UserResultDto> response = await user.Read(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogError($"User retrieval failed: {response}");
            return HandlerResult(Result<string>.Failure("Read Failed"));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Get all user information
    /// </summary>
    /// <returns name="ActionResult">Http Response with list of objects "UserResultDto"</returns>
    [Authorize("ManagerOnly")]
    [HttpGet]
    public async Task<ActionResult<Result<DbOperationResult<UserResultDto>>>> GetAllUser()
    {
        try
        {
            DbOperationResult<UserResultDto> response = await user.Read();

            return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Delete a user from table "User"
    /// </summary>
    /// <param name="id">The integer id of a user</param>
    /// <returns name="ActionResult">Http Response with list of objects "UserResultDto"</returns>
    [Authorize("ManagerOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<DbOperationResult<List<UserResultDto>>>>> DeleteUser(int id)
    {
        try
        {
            DbOperationResult<UserResultDto> response = await user.Delete(id);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogDebug($"Delete Failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Delete Failed."));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    [Authorize("ManagerOnly")]
    [HttpPost("update")]
    public async Task<ActionResult<Result<DbOperationResult<UserResultDto>>>> UpdateUser(UserQueryDto queryDto)
    {
        try
        {
            DbOperationResult<UserResultDto> response = await user.Update(queryDto);

            if (response.amount > 0)
            {
                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogDebug($"Update Failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Update Failed."));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }
    
    /// <summary>
    /// Assign User to a Role
    /// </summary>
    /// <param name="userId">The integer id of a user</param>
    /// <param name="roleId">The integer id of a role</param>
    /// <returns name="ActionResult">Http Response with list of objects "UserResultDto"</returns>
    [Authorize("ManagerOnly")]
    [HttpPost("assign-role/{userId}/{roleId}")]
    public async Task<ActionResult<Result<DbOperationResult<List<UserResultDto>>>>> AssignRole(int userId,
                                                                                               int roleId)
    {
        try
        {
            DbOperationResult<UserResultDto> response = new DbOperationResult<UserResultDto>();
            DbOperationResult<RoleResultDto> roleResult = await role.Read(
                                                                                new RoleQueryDto()
                                                                                {
                                                                                    Id = roleId
                                                                                });

            if (roleResult.amount > 0)
            {
                response = await user.AssignRole(
                                                 userId,
                                                 roleResult.resultDto.First()
                                                           .Name);

                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogDebug($"Assign Role Failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Assign Role Failed."));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }

    /// <summary>
    /// Remove a Role from a user
    /// </summary>
    /// <param name="userId">The integer id of a user</param>
    /// <param name="roleId">The integer id of a role</param>
    /// <returns name="ActionResult">Http Response with list of objects "UserResultDto"</returns>
    [Authorize("ManagerOnly")]
    [HttpPost("remove-role/{userId}/{roleId}")]
    public async Task<ActionResult<Result<DbOperationResult<List<UserResultDto>>>>> RemoveRole(int userId,
                                                                                               int roleId)
    {
        try
        {
            DbOperationResult<UserResultDto> response = new DbOperationResult<UserResultDto>();
            DbOperationResult<RoleResultDto> roleResult = await role.Read(
                                                                                new RoleQueryDto()
                                                                                {
                                                                                    Id = roleId
                                                                                });

            if (roleResult.amount > 0)
            {
                response = await user.RemoveRole(
                                                 userId,
                                                 roleResult.resultDto.First()
                                                           .Name);

                return HandlerResult(Result<DbOperationResult<UserResultDto>>.Success(response));
            }

            logger.LogDebug($"Remove Role Failed: {JsonConvert.SerializeObject(response)}");
            return HandlerResult(Result<string>.Failure("Remove Role Failed."));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }
}