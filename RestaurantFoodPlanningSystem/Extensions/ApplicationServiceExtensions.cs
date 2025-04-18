using System.Text;
using Application.BusinessLogic.FoodItemLogic;
using Application.BusinessLogic.MenuItemFoodItemLogic;
using Application.BusinessLogic.MenuItemLogic;
using Application.BusinessLogic.MenuLogic;
using Application.BusinessLogic.OrderItemLogic;
using Application.BusinessLogic.OrderLogic;
using Application.BusinessLogic.RoleLogic;
using Application.BusinessLogic.TypeLogic;
using Application.BusinessLogic.UnitLogic;
using Application.BusinessLogic.UserLogic;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantFoodPlanningSystem.Services;

namespace RestaurantFoodPlanningSystem.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
                                                            IConfiguration          config)
    {
        services.AddHealthChecks();
        
        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"));
        
        services.AddDbContext<RFPSDbContext>(
                                             opt =>
                                             {
                                                 //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
                                                 opt.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
                                             });
        
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        
        
        services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<RFPSDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                          options =>
                          {
                              options.TokenValidationParameters = new TokenValidationParameters()
                                                                  {
                                                                      ValidateIssuerSigningKey = true,
                                                                      ValidateLifetime = true,
                                                                      ValidateIssuer = true,
                                                                      ValidIssuer = config["Issuer"],
                                                                      ValidateAudience = true,
                                                                      ValidAudiences = config["Audience"]?.Split(","),
                                                                      IssuerSigningKey =
                                                                          new SymmetricSecurityKey(
                                                                                                   Encoding.UTF8
                                                                                                           .GetBytes(
                                                                                                                     Environment.GetEnvironmentVariable("TOKEN")
                                                                                                                    )),
                                                                      RequireExpirationTime = true,
                                                                      ClockSkew             = TimeSpan.FromHours(3)
                                                                  };
                          });


        services.AddAuthorization(
                                  options =>
                                  {
                                      options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
                                      options.AddPolicy("StaffAndManager", policy => policy.RequireRole("Staff", "Manager"));
                                  });
        
        services.AddLogging(
                            logging =>
                            {
                                logging.AddConsole();
                                logging.AddDebug();
                                logging.SetMinimumLevel(LogLevel.Information);
                            });

        services.AddTransient<IMenu, MenuImp>();
        services.AddTransient<IMenuItem, MenuItemImp>();
        services.AddTransient<IMenuItemFoodItem, MenuItemFoodItemImp>();
        services.AddTransient<IFoodItem, FoodItemImp>();
        services.AddTransient<IType, TypeImp>();
        services.AddTransient<IUnit, UnitImp>();
        services.AddTransient<IUser, UserImp>();
        services.AddTransient<IRole, RoleImp>();
        services.AddTransient<IOrder, OrderImp>();
        services.AddTransient<IOrderItem, OrderItemImp>();
        services.AddScoped<TokenService>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationHandler>();

        return services;
    }
}