using ExpenseTracker.Api.Models;
using System;
using System.Collections.Generic;

namespace ExpenseTracker.Api.Services
{
    public interface IExpenseService
    {
        void AddExpense(Expense expense);
        List<Expense> GetAllExpenses();
        Expense GetExpenseById(Guid id);
        decimal GetTotalSpending();
        void SaveExpenses();
        void LoadExpenses();
        bool DeleteExpense(Guid id);
        bool UpdateExpense(Guid id, decimal? amount, string category, string description);
        List<Expense> GetExpensesByCategory(string category);
        List<string> GetAllCategories();
        
    }
}