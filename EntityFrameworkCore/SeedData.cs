using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Type = Domain.Type;

namespace EntityFrameworkCore;

public class SeedData
{
    public static async Task CreateSeedData(RoleManager<Role> roleManager,
                                            UserManager<User> userManager,
                                            RFPSDbContext     context,
                                            ILogger<SeedData>
                                                logger)
    {
        if (roleManager == null)
        {
            throw new ArgumentNullException(nameof(roleManager));
        }

        if (userManager == null)
        {
            throw new ArgumentNullException(nameof(userManager));
        }

        if (!roleManager.Roles.Any())
        {
            List<Role> roles = new List<Role>()
                               {
                                   new Role(
                                            "Manager",
                                            "Manager",
                                            DateTime.Now),
                                   new Role(
                                            "Staff",
                                            "Staff",
                                            DateTime.Now),
                               };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }

        if (roleManager.Roles.Any() && !userManager.Users.Any())
        {
            List<User> users = new List<User>()
                               {
                                   new User(
                                            "Kathy",
                                            "kathy"),
                                   new User(
                                            "Harus",
                                            "harus"),
                               };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(
                                                 user,
                                                 "Manager");
            }

            List<User> staffs = new List<User>()
                                {
                                    new User(
                                             "KathyStaff",
                                             "kathy_staff"),
                                    new User(
                                             "HarusStaff",
                                             "harus_staff"),
                                };

            foreach (var staff in staffs)
            {
                await userManager.CreateAsync(staff);
                await userManager.AddToRoleAsync(
                                                 staff,
                                                 "Staff");
            }
        }

        if (!context.Type.Any())
        {
            List<Type> types = new List<Type>()
                               {
                                   new Type()
                                   {
                                       Name = "Meat"
                                   },
                                   new Type()
                                   {
                                       Name = "Vegetable"
                                   },
                                   new Type()
                                   {
                                       Name = "Sauce"
                                   }
                               };

            context.Type.AddRange(types);
            logger.LogDebug($"Type insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.Unit.Any())
        {
            List<Unit> units =
            [
                new Unit()
                {
                    Name = "milliliter"
                },

                new Unit()
                {
                    Name = "liter"
                },

                new Unit()
                {
                    Name = "gram"
                },

                new Unit()
                {
                    Name = "kilogram"
                }
            ];

            context.Unit.AddRange(units);
            logger.LogDebug($"Unit insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.FoodItem.Any() && context.Unit.Any() && context.Type.Any())
        {
            List<FoodItem> foodItems =
            [
                new FoodItem()
                {
                    Name     = "Beef",
                    Quantity = 10,
                    Type_Id  = 1,
                    Unit_Id  = 4
                },

                new FoodItem()
                {
                    Name     = "Carrot",
                    Quantity = 10,
                    Type_Id  = 2,
                    Unit_Id  = 4
                },

                new FoodItem()
                {
                    Name     = "Soy Sauce",
                    Quantity = 10,
                    Type_Id  = 3,
                    Unit_Id  = 2
                },

                new FoodItem()
                {
                    Name     = "Chicken",
                    Quantity = 20,
                    Type_Id  = 1,
                    Unit_Id  = 4
                }
            ];
            context.FoodItem.AddRange(foodItems);
            logger.LogDebug($"FoodItem insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.MenuItem.Any())
        {
            List<MenuItem> menuItems =
            [
                new MenuItem()
                {
                    Name = "BBQ Beef"
                },

                new MenuItem()
                {
                    Name = "Roasted Chicken"
                },

                new MenuItem()
                {
                    Name = "Steamed Chicken"
                }
            ];

            context.MenuItem.AddRange(menuItems);
            logger.LogDebug($"MenuItem insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.Menu.Any() && context.MenuItem.Any())
        {
            List<Menu> menus =
            [
                new Menu()
                {
                    MenuItem_Id = 1,
                    Date        = DateTime.Parse("2024-06-23")
                },

                new Menu()
                {
                    MenuItem_Id = 2,
                    Date        = DateTime.Parse("2024-06-23")
                },

                new Menu()
                {
                    MenuItem_Id = 3,
                    Date        = DateTime.Parse("2024-06-22")
                }
            ];

            context.Menu.AddRange(menus);
            logger.LogDebug($"Menu insertion result: {await context.SaveChangesAsync()}");
        }

        TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(
                                                             DateTime.UtcNow,
                                                             zoneInfo);
        if (!context.Menu.Any(x => x.Date == localDate) && context.MenuItem.Any())
        {
            Menu menu = new Menu()
                        {
                            MenuItem_Id = 2,
                            Date        = localDate
                        };

            context.Menu.Add(menu);
            logger.LogDebug($"Menu insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.MenuItemFoodItem.Any() && context.FoodItem.Any() && context.MenuItem.Any())
        {
            List<MenuItemFoodItem> menuItemFoodItems =
            [
                new MenuItemFoodItem()
                {
                    MenuItem_Id = 1,
                    FoodItem_Id = 1,
                    Consumption = 2
                },
                new MenuItemFoodItem()
                {
                    MenuItem_Id = 1,
                    FoodItem_Id = 3,
                    Consumption = 1
                },
                new MenuItemFoodItem()
                {
                    MenuItem_Id = 2,
                    FoodItem_Id = 4,
                    Consumption = 2
                },
                new MenuItemFoodItem()
                {
                    MenuItem_Id = 3,
                    FoodItem_Id = 4,
                    Consumption = 2
                },
                new MenuItemFoodItem()
                {
                    MenuItem_Id = 3,
                    FoodItem_Id = 2,
                    Consumption = 2
                }
            ];

            context.MenuItemFoodItem.AddRange(menuItemFoodItems);
            logger.LogDebug($"MenuItemFoodItem insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.Order.Any() && context.MenuItem.Any())
        {
            List<Order> order =
            [
                new Order()
                {
                    IsCanceled = false,
                },
                new Order()
                {
                    IsCanceled = true,
                }
            ];
            context.Order.AddRange(order);
            logger.LogDebug($"Order insertion result: {await context.SaveChangesAsync()}");
        }

        if (!context.OrderItem.Any() && context.Order.Any())
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            List<int> orderIds = context
                                 .Order
                                 .Select(x => x.Id)
                                 .ToList();

            List<int> menuItemIds = context
                                    .MenuItem
                                    .Select(x => x.Id)
                                    .ToList();


            foreach (int id in orderIds)
            {
                int items = Random.Shared.Next(
                                               1,
                                               4);

                for (int i = 0; i < items; i++)
                {
                    orderItems.Add(
                                   new OrderItem()
                                   {
                                       OrderId = id,
                                       MenuItemId = menuItemIds[Random.Shared.Next(
                                                                                   0,
                                                                                   3)]
                                   });
                }
            }


            context.OrderItem.AddRange(orderItems);
            logger.LogDebug($"Order insertion result: {await context.SaveChangesAsync()}");
        }
    }
}