using ExpenseTracker.API.Models; // Ensure this is API

namespace ExpenseTracker.API.Services // <--- CHANGE THIS from .Api to .API
{
    public interface IAuthService
    {
        Guid GetUserId();
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