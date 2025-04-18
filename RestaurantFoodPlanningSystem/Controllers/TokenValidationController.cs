using Application;
using Application.Dtos.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantFoodPlanningSystem.Services;

namespace RestaurantFoodPlanningSystem.Controllers;

public class TokenValidationController(
    ILogger<TokenValidationController> logger,
    TokenService                       tokenService) : BaseApiController(logger)
{
    /// <summary>
    /// Token Validation
    /// </summary>
    /// <param name="tokenQueryDto">The existing token</param>
    /// <returns name="ActionResult">Http Response with Result object</returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<Result<DbOperationResult<TokenResultDto>>>> TokenValidation(TokenQueryDto tokenQueryDto)
    {
        try
        {
            DbOperationResult<TokenResultDto> result = new DbOperationResult<TokenResultDto>();
            TokenResultDto                    dto    = new TokenResultDto();
            if (tokenService.ValidateToken(tokenQueryDto.token))
            {
                dto.valid = true;
                result.resultDto = new List<TokenResultDto>()
                                   {
                                       dto
                                   };
                result.amount = 1;
                return HandlerResult(Result<DbOperationResult<TokenResultDto>>.Success(result));
            }

            dto.valid = false;
            result.resultDto = new List<TokenResultDto>()
                               {
                                   dto
                               };
            result.amount = 1;
            return HandlerResult(Result<DbOperationResult<TokenResultDto>>.Success(result));
        }
        catch (Exception e)
        {
            logger.LogError(JsonConvert.SerializeObject(e));
            return HandlerResult(Result<string>.Failure(e.Message));
        }
    }
}