using System.Text;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Newtonsoft.Json;

namespace RestaurantFoodPlanningSystem.Extensions;

public class AuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate           next,
                                  HttpContext               context,
                                  AuthorizationPolicy       policy,
                                  PolicyAuthorizationResult authorizeResult)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
                       {
                           Message = ""
                       };

        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            response = new
                       {
                           Message = "Unauthorized access."
                       };
            await context.Response.WriteAsJsonAsync(response);
            return;
        }
        else if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            response = new
                       {
                           Message = "Forbidden access."
                       };
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _defaultHandler.HandleAsync(
                                    next,
                                    context,
                                    policy,
                                    authorizeResult);
    }
}