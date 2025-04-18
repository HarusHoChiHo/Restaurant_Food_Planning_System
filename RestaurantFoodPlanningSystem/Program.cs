using System.Reflection;
using Domain;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestaurantFoodPlanningSystem.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

// Add services to the container.

builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
                       {
                           options.SerializerSettings.ReferenceLoopHandling =
                               Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                       });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
                               {
                                   option.AddSecurityDefinition(
                                                                "Bearer",
                                                                new OpenApiSecurityScheme() {
                                                                                                Name         = "Authorization",
                                                                                                Type         = SecuritySchemeType.ApiKey,
                                                                                                Scheme       = "Bearer",
                                                                                                BearerFormat = "JWT",
                                                                                                In           = ParameterLocation.Header,
                                                                                                Description  = "Add bearer token"
                                                                                            });

                                   option.AddSecurityRequirement(
                                                                 new OpenApiSecurityRequirement() {
                                                                                                      {
                                                                                                          new OpenApiSecurityScheme() {
                                                                                                                                          Reference = new OpenApiReference() {
                                                                                                                                                                                 Type = ReferenceType
                                                                                                                                                                                     .SecurityScheme,
                                                                                                                                                                                 Id = "Bearer"
                                                                                                                                                                             }
                                                                                                                                      },
                                                                                                          new string[] {
                                                                                                                       }
                                                                                                      }
                                                                                                  });

                                   string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                                   string xmlPath = Path.Combine(
                                                                 AppContext.BaseDirectory,
                                                                 xmlFile);

                                   option.IncludeXmlComments(xmlPath);


                                   option.SwaggerDoc(
                                                     "v1",
                                                     new OpenApiInfo {
                                                                         Title   = "Restaurant Food Planning System",
                                                                         Version = "v1"
                                                                     });
                               });
builder.Services.AddApplicationServices(builder.Configuration);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
                         {
                             options.AddPolicy(
                                               name: "CORS",
                                               policy =>
                                               {
                                                   policy
                                                       .WithOrigins(
                                                                    builder.Configuration.GetSection("AllowedHosts")
                                                                           .Value)
                                                       .AllowAnyHeader()
                                                       .AllowAnyMethod();
                                               });
                             //Console.WriteLine(builder.Configuration.GetSection("AllowedHosts").Value);
                         });

var app = builder.Build();

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await using var context = services.GetRequiredService<RFPSDbContext>();
        context.Database.Migrate();

        var optionsBuilder = new DbContextOptionsBuilder<RFPSDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // await using (var dbContext = new RFPSDbContext(optionsBuilder.Options))
        // {
        //     var migrationService = new MigrationService(
        //                                                 dbContext,
        //                                                 services.GetRequiredService<ILogger<MigrationService>>(),
        //                                                 builder.Configuration);
        //     await migrationService.Migration(
        //                                      null,
        //                                      null,
        //                                      true);
        // }

        await using var createSeedDatacontext = services.GetRequiredService<RFPSDbContext>();
        await SeedData.CreateSeedData(
                                      services.GetRequiredService<RoleManager<Role>>(),
                                      services.GetRequiredService<UserManager<User>>(),
                                      createSeedDatacontext,
                                      services.GetRequiredService<ILogger<SeedData>>());
    }
    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(
                        e,
                        "An error occured during migration.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint(
                                            "/swagger/v1/swagger.json",
                                            "JWTAuthoDemo v1"));
}

app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();