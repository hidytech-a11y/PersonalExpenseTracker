using ExpenseTracker.Models;
using System;
using System.Collections.Generic;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        void AddExpense(Expense expense);
        List<Expense> GetAllExpenses();
        decimal GetTotalSpending();
        void SaveExpenses();
        void LoadExpenses();
        bool DeleteExpense(Guid id);
        bool UpdateExpense(Guid id, decimal? amount, string category, string description);
        List<Expense> GetExpensesByCategory(string category);
        List<string> GetAllCategories();
    }
}