using AuthService.Models;

namespace AuthService.Services
{
    public interface IUserService
    {
        User? Register(string username, string email, string password);
        string? Authenticate(string username, string password);
    }
}