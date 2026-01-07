using ExpenseTracker.Api.Models;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string name, string email, string password);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(Guid userId);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
    }
}