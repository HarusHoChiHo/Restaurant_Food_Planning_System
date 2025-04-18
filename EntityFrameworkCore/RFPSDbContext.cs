using System.Reflection.Emit;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore;

public class RFPSDbContext : IdentityDbContext<User, Role, int>
{
    protected RFPSDbContext()
    {
    }
    
    public RFPSDbContext(DbContextOptions options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        /* BEG: Define primary key for each table */

        //modelBuilder.Entity<User>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<Menu>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<MenuItem>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<FoodItem>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<Unit>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<Domain.Type>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<MenuItemFoodItem>(
                                              x => x.HasKey(
                                                            pk => new
                                                                  {
                                                                      pk.FoodItem_Id,
                                                                      pk.MenuItem_Id
                                                                  }));
        modelBuilder.Entity<Order>(x => x.HasKey(pk => pk.Id));
        modelBuilder.Entity<OrderItem>(x => x.HasKey(pk => pk.Id));

        /* END: Define primary key for each table */

        /* BEG: Define foreign key and relationship for each table */

        modelBuilder
            .Entity<Menu>()
            .HasOne<MenuItem>(menu => menu.MenuItem)
            .WithMany(menuItem => menuItem.Menus)
            .HasForeignKey(menu => menu.MenuItem_Id);

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .HasOne<MenuItem>(mifi => mifi.MenuItem)
            .WithMany(menuItem => menuItem.MenuItemFoodItems)
            .HasForeignKey(mifi => mifi.MenuItem_Id)
            .HasPrincipalKey(menuItem => menuItem.Id);

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .HasOne<FoodItem>(mifi => mifi.FoodItem)
            .WithMany(foodItem => foodItem.MenuItemFoodItems)
            .HasForeignKey(mifi => mifi.FoodItem_Id)
            .HasPrincipalKey(foodItem => foodItem.Id);
        
        modelBuilder
            .Entity<FoodItem>()
            .HasOne<Unit>(foodItem => foodItem.Unit)
            .WithMany(unit => unit.FoodItems)
            .HasForeignKey(foodItem => foodItem.Unit_Id);

        modelBuilder
            .Entity<FoodItem>()
            .HasOne<Domain.Type>(foodItem => foodItem.Type)
            .WithMany(type => type.FoodItems)
            .HasForeignKey(foodItem => foodItem.Type_Id);

        modelBuilder
            .Entity<MenuItem>()
            .HasMany(menuItem => menuItem.OrderItems)
            .WithOne(orderItem => orderItem.MenuItem)
            .HasForeignKey(menuItem => menuItem.MenuItemId);

        modelBuilder
            .Entity<Order>()
            .HasMany(order => order.OrderItems)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(orderItem => orderItem.OrderId);

        /* END: Define foreign key and relationship for each table */

        /* BEG: Define columns for each table */
        modelBuilder
            .Entity<Menu>()
            .Property(prop => prop.Id)
            .UseIdentityColumn()
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<Menu>()
            .Property(prop => prop.Date)
            .IsRequired()
            .HasColumnType("date")
            .HasColumnOrder(1);

        modelBuilder
            .Entity<MenuItem>()
            .Property(prop => prop.Id)
            .UseIdentityColumn()
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<MenuItem>()
            .Property(prop => prop.Name)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .Property(prop => prop.MenuItem_Id)
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .Property(prop => prop.FoodItem_Id)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .Property(prop => prop.Consumption)
            .IsRequired()
            .HasColumnOrder(2);

        modelBuilder
            .Entity<FoodItem>()
            .Property(prop => prop.Id)
            .UseIdentityColumn()
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<FoodItem>()
            .Property(prop => prop.Name)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder
            .Entity<FoodItem>()
            .Property(prop => prop.Quantity)
            .IsRequired()
            .HasColumnOrder(2);

        modelBuilder
            .Entity<FoodItem>()
            .Property(prop => prop.Unit_Id)
            .IsRequired()
            .HasColumnOrder(3);

        modelBuilder
            .Entity<FoodItem>()
            .Property(prop => prop.Type_Id)
            .IsRequired()
            .HasColumnOrder(4);

        modelBuilder
            .Entity<Unit>()
            .Property(prop => prop.Id)
            .UseIdentityColumn()
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<Unit>()
            .Property(prop => prop.Name)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder
            .Entity<Domain.Type>()
            .Property(prop => prop.Id)
            .UseIdentityColumn()
            .IsRequired()
            .HasColumnOrder(0);

        modelBuilder
            .Entity<Domain.Type>()
            .Property(prop => prop.Name)
            .IsRequired()
            .HasColumnOrder(1);

        modelBuilder
            .Entity<Order>()
            .Property(prop => prop.Id)
            .IsRequired()
            .HasColumnOrder(0);
        
        modelBuilder
            .Entity<Order>()
            .Property(prop => prop.IsCanceled)
            .HasColumnOrder(1)
            .HasDefaultValue(false);
        
        modelBuilder
            .Entity<OrderItem>()
            .Property(prop => prop.Id)
            .IsRequired()
            .HasColumnOrder(0);
        
        modelBuilder
            .Entity<OrderItem>()
            .Property(prop => prop.OrderId)
            .IsRequired()
            .HasColumnOrder(1);
        
        modelBuilder
            .Entity<OrderItem>()
            .Property(prop => prop.MenuItemId)
            .IsRequired()
            .HasColumnOrder(2);

        /* END: Define columns for each table */

        /* BEG: Define Unique Columns */

        modelBuilder
            .Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder
            .Entity<MenuItem>()
            .HasIndex(mi => mi.Name)
            .IsUnique();
        
        /* END: Define Unique Columns */

        /* BEG: Define constraints */

        modelBuilder
            .Entity<MenuItemFoodItem>()
            .ToTable(
                     table => table.HasCheckConstraint(
                                                       "CK_Consumption",
                                                       "\"Consumption\" > 0"));
        /* END: Define constraints */
    }

    //public DbSet<User>             User             { get; set; }
    public DbSet<Menu>             Menu             { get; set; }
    public DbSet<MenuItem>         MenuItem         { get; set; }
    public DbSet<FoodItem>         FoodItem         { get; set; }
    public DbSet<Domain.Type>      Type             { get; set; }
    public DbSet<Unit>             Unit             { get; set; }
    public DbSet<MenuItemFoodItem> MenuItemFoodItem { get; set; }
    public DbSet<Order>            Order            { get; set; }
    public DbSet<OrderItem>        OrderItem        { get; set; }
}