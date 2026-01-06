using ExpenseTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private List<Expense> _expenses;
        private readonly string _filePath = "expenses.json";

        public ExpenseService()
        {
            _expenses = new List<Expense>();
            LoadExpenses();
        }

        public void AddExpense(Expense expense)
        {
            _expenses.Add(expense);
            SaveExpenses();
        }

        public List<Expense> GetAllExpenses()
        {
            return _expenses;
        }

        public decimal GetTotalSpending()
        {
            return _expenses.Sum(e => e.Amount);
        }

        public void SaveExpenses()
        {
            var json = JsonSerializer.Serialize(_expenses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void LoadExpenses()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _expenses = JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
            }
        }

        public bool DeleteExpense(Guid id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _expenses.Remove(expense);
                SaveExpenses();
                return true;
            }
            return false;
        }

        public bool UpdateExpense(Guid id, decimal? amount, string category, string description)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                if (amount.HasValue)
                    expense.Amount = amount.Value;
                if (!string.IsNullOrWhiteSpace(category))
                    expense.Category = category;
                if (!string.IsNullOrWhiteSpace(description))
                    expense.Description = description;

                SaveExpenses();
                return true;
            }
            return false;
        }

        public List<Expense> GetExpensesByCategory(string category)
        {
            return _expenses
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.Date)
                .ToList();
        }

        public List<string> GetAllCategories()
        {
            return _expenses
                .Select(e => e.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }
    }
}