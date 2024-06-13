﻿// <auto-generated />
using System;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityFrameworkCore.Migrations
{
    [DbContext(typeof(RFPSDbContext))]
    partial class RFPSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.FoodItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("Type_Id")
                        .HasColumnType("int")
                        .HasColumnOrder(4);

                    b.Property<int>("Unit_Id")
                        .HasColumnType("int")
                        .HasColumnOrder(3);

                    b.HasKey("Id");

                    b.HasIndex("Type_Id");

                    b.HasIndex("Unit_Id");

                    b.ToTable("FoodItem");
                });

            modelBuilder.Entity("Domain.Menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("smalldatetime")
                        .HasColumnOrder(1);

                    b.Property<int>("MenuItem_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MenuItem_Id");

                    b.ToTable("Menu");
                });

            modelBuilder.Entity("Domain.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.ToTable("MenuItem");
                });

            modelBuilder.Entity("Domain.MenuItemFoodItem", b =>
                {
                    b.Property<int>("FoodItem_Id")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("MenuItem_Id")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("Consumption")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.HasKey("FoodItem_Id", "MenuItem_Id");

                    b.HasIndex("MenuItem_Id");

                    b.ToTable("MenuItemFoodItem", t =>
                        {
                            t.HasCheckConstraint("CK_Consumption", "[Consumption] > 0");
                        });
                });

            modelBuilder.Entity("Domain.Type", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.ToTable("Type");
                });

            modelBuilder.Entity("Domain.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.ToTable("Unit");
                });

            modelBuilder.Entity("Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(2);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Domain.FoodItem", b =>
                {
                    b.HasOne("Domain.Type", "Type")
                        .WithMany("FoodItems")
                        .HasForeignKey("Type_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Unit", "Unit")
                        .WithMany("FoodItems")
                        .HasForeignKey("Unit_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Domain.Menu", b =>
                {
                    b.HasOne("Domain.MenuItem", "MenuItem")
                        .WithMany("Menus")
                        .HasForeignKey("MenuItem_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("Domain.MenuItemFoodItem", b =>
                {
                    b.HasOne("Domain.FoodItem", "FoodItem")
                        .WithMany("MenuItemFoodItems")
                        .HasForeignKey("FoodItem_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.MenuItem", "MenuItem")
                        .WithMany("MenuItemFoodItems")
                        .HasForeignKey("MenuItem_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FoodItem");

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("Domain.FoodItem", b =>
                {
                    b.Navigation("MenuItemFoodItems");
                });

            modelBuilder.Entity("Domain.MenuItem", b =>
                {
                    b.Navigation("MenuItemFoodItems");

                    b.Navigation("Menus");
                });

            modelBuilder.Entity("Domain.Type", b =>
                {
                    b.Navigation("FoodItems");
                });

            modelBuilder.Entity("Domain.Unit", b =>
                {
                    b.Navigation("FoodItems");
                });
#pragma warning restore 612, 618
        }
    }
}
