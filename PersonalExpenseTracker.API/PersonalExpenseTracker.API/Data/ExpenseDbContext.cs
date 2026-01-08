using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<RecurringExpense> RecurringExpenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Expense entity
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Date).IsRequired();
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.UserId);
            });

            // Budget entity
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.MonthlyLimit).HasColumnType("decimal(18,2)");
                entity.Property(b => b.Category).IsRequired().HasMaxLength(50);
                entity.HasIndex(b => new { b.Category, b.Month, b.Year, b.UserId }).IsUnique();
            });

            // Recurring Expense entity
            modelBuilder.Entity<RecurringExpense>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");
                entity.Property(r => r.Category).IsRequired().HasMaxLength(50);
                entity.Property(r => r.Description).IsRequired().HasMaxLength(200);
                entity.HasIndex(r => r.UserId);
            });
        }
    }
}