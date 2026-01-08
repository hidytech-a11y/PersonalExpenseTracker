using System;
using ExpenseTracker.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExpenseTracker.API.Migrations
{
    [DbContext(typeof(ExpenseDbContext))]
    [Migration("20260107081338_AddBudgets")]
    partial class AddBudgets
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ExpenseTracker.API.Models.Budget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<decimal>("MonthlyLimit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Category", "Month", "Year")
                        .IsUnique();

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("ExpenseTracker.Api.Models.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("Category");

                    b.HasIndex("Date");

                    b.ToTable("Expenses");

                    b.HasData(
                        new
                        {
                            Id = new Guid("30143a5c-9578-4e1c-b223-3873abde5e45"),
                            Amount = 25.50m,
                            Category = "Food",
                            Date = new DateTime(2026, 1, 2, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6309),
                            Description = "Lunch at cafe"
                        },
                        new
                        {
                            Id = new Guid("e224d707-a1f0-448a-8039-548a5059662f"),
                            Amount = 50.00m,
                            Category = "Transport",
                            Date = new DateTime(2026, 1, 4, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6319),
                            Description = "Gas for car"
                        },
                        new
                        {
                            Id = new Guid("cdb4283c-3817-456a-b649-be14d4b9cf01"),
                            Amount = 120.00m,
                            Category = "Bills",
                            Date = new DateTime(2026, 1, 6, 8, 13, 36, 60, DateTimeKind.Utc).AddTicks(6338),
                            Description = "Internet bill"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
