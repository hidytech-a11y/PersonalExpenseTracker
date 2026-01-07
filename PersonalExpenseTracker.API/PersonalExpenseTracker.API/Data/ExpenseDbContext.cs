using ExpenseTracker.Api.Models;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Expense entity
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Date)
                    .IsRequired();

                // Add index for better query performance
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Date);
            });

            // Configure Budget entity
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.MonthlyLimit)
                    .HasColumnType("decimal(18,2)");

                entity.Property(b => b.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                // Add index for better query performance
                // Unique constraint: one budget per category per month
                entity.HasIndex(b => new { b.Category, b.Month, b.Year })
                    .IsUnique();
            });

            // Seed some initial data (optional)
            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 25.50m,
                    Category = "Food",
                    Description = "Lunch at cafe",
                    Date = DateTime.UtcNow.AddDays(-5)
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 50.00m,
                    Category = "Transport",
                    Description = "Gas for car",
                    Date = DateTime.UtcNow.AddDays(-3)
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Amount = 120.00m,
                    Category = "Bills",
                    Description = "Internet bill",
                    Date = DateTime.UtcNow.AddDays(-1)
                }
            );
        }
    }
}