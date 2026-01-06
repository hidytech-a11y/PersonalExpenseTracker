using ExpenseTracker.Models;
using ExpenseTracker.Services;
using System;
using System.Linq;

namespace ExpenseTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            IExpenseService expenseService = new ExpenseService();
            bool running = true;

            // Welcome Header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================");
            Console.WriteLine("  PERSONAL EXPENSE TRACKER");
            Console.WriteLine("=================================\n");
            Console.ResetColor();

            while (running)
            {
                // Menu
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- MAIN MENU ---");
                Console.ResetColor();

                Console.WriteLine("1. Add Expense");
                Console.WriteLine("2. View All Expenses");
                Console.WriteLine("3. Filter by Category");
                Console.WriteLine("4. Edit Expense");
                Console.WriteLine("5. Delete Expense");
                Console.WriteLine("6. View Total Spending");
                Console.WriteLine("7. Exit");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nChoose an option (1-7): ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddExpense(expenseService);
                        break;
                    case "2":
                        ViewAllExpenses(expenseService);
                        break;
                    case "3":
                        FilterByCategory(expenseService);
                        break;
                    case "4":
                        EditExpense(expenseService);
                        break;
                    case "5":
                        DeleteExpense(expenseService);
                        break;
                    case "6":
                        ViewTotalSpending(expenseService);
                        break;
                    case "7":
                        running = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n👋 Thanks for using Expense Tracker. Goodbye!");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n❌ Invalid option. Please choose 1-6.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        static void AddExpense(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== ADD NEW EXPENSE ==========");
            Console.ResetColor();

            try
            {
                Console.Write("Amount ($): ");
                decimal amount = decimal.Parse(Console.ReadLine());

                Console.Write("Category (Food/Transport/Entertainment/Bills/Other): ");
                string category = Console.ReadLine();

                Console.Write("Description: ");
                string description = Console.ReadLine();

                var expense = new Expense
                {
                    Amount = amount,
                    Category = category,
                    Description = description
                };

                service.AddExpense(expense);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✅ Expense added successfully!");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ Error: Please enter a valid number for amount.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ViewAllExpenses(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== ALL EXPENSES ==========");
            Console.ResetColor();

            var expenses = service.GetAllExpenses();

            if (expenses.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📭 No expenses recorded yet. Start adding some!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nTotal Expenses: {expenses.Count}\n");
            Console.ResetColor();

            for (int i = 0; i < expenses.Count; i++)
            {
                var expense = expenses[i];

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{i + 1}] ");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"📅 Date: {expense.Date:yyyy-MM-dd HH:mm}");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"    💵 Amount: ${expense.Amount:F2}");
                Console.ResetColor();

                Console.Write($"    📁 Category: ");
                Console.ForegroundColor = GetCategoryColor(expense.Category);
                Console.WriteLine(expense.Category);
                Console.ResetColor();

                Console.WriteLine($"    📝 Description: {expense.Description}");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("-------------------------------------");
                Console.ResetColor();
            }
        }

        static void EditExpense(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== EDIT EXPENSE ==========");
            Console.ResetColor();

            var expenses = service.GetAllExpenses();

            if (expenses.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📭 No expenses to edit!");
                Console.ResetColor();
                return;
            }

            // Show numbered list
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nYou have {expenses.Count} expense(s):\n");
            Console.ResetColor();

            for (int i = 0; i < expenses.Count; i++)
            {
                var expense = expenses[i];
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{i + 1}] ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"${expense.Amount:F2} ");
                Console.ForegroundColor = GetCategoryColor(expense.Category);
                Console.Write($"({expense.Category}) ");
                Console.ResetColor();
                Console.WriteLine($"- {expense.Description}");
            }

            // Get user choice
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nEnter the number of the expense to edit (or 0 to cancel): ");
            Console.ResetColor();

            try
            {
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("❌ Edit cancelled.");
                    Console.ResetColor();
                    return;
                }

                if (choice < 1 || choice > expenses.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Invalid number. Please choose between 1 and {expenses.Count}.");
                    Console.ResetColor();
                    return;
                }

                var expenseToEdit = expenses[choice - 1];

                // Show current values and get new ones
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- Current Expense Details ---");
                Console.ResetColor();
                Console.WriteLine($"Amount: ${expenseToEdit.Amount:F2}");
                Console.WriteLine($"Category: {expenseToEdit.Category}");
                Console.WriteLine($"Description: {expenseToEdit.Description}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nEnter new values (or press Enter to keep current value):");
                Console.ResetColor();

                // Amount
                Console.Write($"New Amount (current: ${expenseToEdit.Amount:F2}): $");
                string amountInput = Console.ReadLine();
                decimal? newAmount = null;
                if (!string.IsNullOrWhiteSpace(amountInput))
                {
                    if (decimal.TryParse(amountInput, out decimal parsedAmount))
                    {
                        newAmount = parsedAmount;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("⚠️  Invalid amount, keeping current value.");
                        Console.ResetColor();
                    }
                }

                // Category
                Console.Write($"New Category (current: {expenseToEdit.Category}): ");
                string newCategory = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newCategory))
                {
                    newCategory = expenseToEdit.Category;
                }

                // Description
                Console.Write($"New Description (current: {expenseToEdit.Description}): ");
                string newDescription = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newDescription))
                {
                    newDescription = expenseToEdit.Description;
                }

                // Confirm changes
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n💾 Save changes? (y/n): ");
                Console.ResetColor();

                string confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    bool success = service.UpdateExpense(
                        expenseToEdit.Id,
                        newAmount,
                        newCategory,
                        newDescription
                    );

                    if (success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n✅ Expense updated successfully!");
                        Console.ResetColor();

                        // Show updated expense
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\n--- Updated Expense ---");
                        Console.ResetColor();
                        var updated = service.GetAllExpenses().FirstOrDefault(e => e.Id == expenseToEdit.Id);
                        if (updated != null)
                        {
                            Console.WriteLine($"Amount: ${updated.Amount:F2}");
                            Console.WriteLine($"Category: {updated.Category}");
                            Console.WriteLine($"Description: {updated.Description}");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n❌ Failed to update expense.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n❌ Changes discarded.");
                    Console.ResetColor();
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ Error: Please enter a valid number.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void DeleteExpense(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== DELETE EXPENSE ==========");
            Console.ResetColor();

            var expenses = service.GetAllExpenses();

            if (expenses.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📭 No expenses to delete!");
                Console.ResetColor();
                return;
            }

            // Show numbered list
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nYou have {expenses.Count} expense(s):\n");
            Console.ResetColor();

            for (int i = 0; i < expenses.Count; i++)
            {
                var expense = expenses[i];
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{i + 1}] ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"${expense.Amount:F2} ");
                Console.ForegroundColor = GetCategoryColor(expense.Category);
                Console.Write($"({expense.Category}) ");
                Console.ResetColor();
                Console.WriteLine($"- {expense.Description}");
            }

            // Get user choice
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nEnter the number of the expense to delete (or 0 to cancel): ");
            Console.ResetColor();

            try
            {
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("❌ Delete cancelled.");
                    Console.ResetColor();
                    return;
                }

                if (choice < 1 || choice > expenses.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Invalid number. Please choose between 1 and {expenses.Count}.");
                    Console.ResetColor();
                    return;
                }

                var expenseToDelete = expenses[choice - 1];

                // Confirmation
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n⚠️  Are you sure you want to delete: ${expenseToDelete.Amount:F2} - {expenseToDelete.Description}? (y/n): ");
                Console.ResetColor();

                string confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    bool success = service.DeleteExpense(expenseToDelete.Id);

                    if (success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n✅ Expense deleted successfully!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n❌ Failed to delete expense.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n❌ Delete cancelled.");
                    Console.ResetColor();
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ Error: Please enter a valid number.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void ViewTotalSpending(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== SPENDING SUMMARY ==========");
            Console.ResetColor();

            decimal total = service.GetTotalSpending();
            var expenses = service.GetAllExpenses();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"💰 Total Expenses: {expenses.Count}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"💸 Total Amount: ${total:F2}");
            Console.ResetColor();

            if (expenses.Count > 0)
            {
                var avgExpense = total / expenses.Count;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"📊 Average per Expense: ${avgExpense:F2}");
                Console.ResetColor();
            }
        }

        static ConsoleColor GetCategoryColor(string category)
        {
            switch (category.ToLower())
            {
                case "food":
                    return ConsoleColor.Green;
                case "transport":
                    return ConsoleColor.Blue;
                case "entertainment":
                    return ConsoleColor.Magenta;
                case "bills":
                    return ConsoleColor.Red;
                case "other":
                    return ConsoleColor.Gray;
                default:
                    return ConsoleColor.White;
            }
        }
        static void FilterByCategory(IExpenseService service)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n========== FILTER BY CATEGORY ==========");
            Console.ResetColor();

            var categories = service.GetAllCategories();

            if (categories.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" No Expenses available! Add some First!");
                Console.ResetColor();
                return;
            }

            // Show availabe categories
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nAvailable Categories({categories.Count}:\n");
            Console.ResetColor();

            for (int i = 0; i < categories.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{i + 1}] ");
                Console.ForegroundColor = GetCategoryColor(categories[i]);
                Console.WriteLine(categories[i]);
                Console.ResetColor();
            }

            // Get user choice
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nEnter category number (or 0 to cancel): ");
            Console.ResetColor();

            try
            {
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Filter cancelled.");
                    Console.ResetColor();
                    return;
                }

                if (choice < 1 || choice > categories.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" Invalid number. Please choose between 1 and {categories.Count}.");
                    Console.ResetColor();
                    return;
                }

                string selectedCategory = categories[choice - 1];
                var filteredExpenses = service.GetExpensesByCategory(selectedCategory);

                // Display filtered expenses result
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n ========={selectedCategory.ToUpper()} EXPENSES ==========");
                Console.ResetColor();

                if (filteredExpenses.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" No expenses found in category: {selectedCategory}");
                    Console.ResetColor();
                    return;
                }

                decimal categoryTotal = filteredExpenses.Sum(e => e.Amount);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nShowing {filteredExpenses.Count} expense(s) | Total: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"${categoryTotal:F2}\n");
                Console.ResetColor();

                for (int i = 0; i < filteredExpenses.Count; i++)
                {
                    var expense = filteredExpenses[i];

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[{i + 1}]  ");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" Date: {expense.Date:yyyy-MM-dd HH:mm}");

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"    Amoun: ${expense.Amount:F2}");
                    Console.ResetColor();

                    Console.WriteLine($"    Description: {expense.Description}");

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("------------------------------------");
                    Console.ResetColor();
                }

                // Show percentage of total spending
                decimal totalSpending = service.GetTotalSpending();
                if (totalSpending > 0)
                {
                    decimal percentage = (categoryTotal / totalSpending) * 100;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n This category represents {percentage:F1}% of your total spending. ");
                    Console.ResetColor();
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Error: Please enter a valid number.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}