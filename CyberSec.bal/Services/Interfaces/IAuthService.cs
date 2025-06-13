using CyberSec.dal.Entities;

namespace CyberSec.bal.Services.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateUserAsync(string username, string password);
    Task<bool> RegisterUserAsync(string username, string password);
    Task<User?> GetUserByUsernameAsync(string username);
}