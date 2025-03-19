using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CyberSec.bal.Services;

public class AuthService : IAuthService
{
    private readonly CyberSecDbContext _dbContext;

    public AuthService(CyberSecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Username == username && u.Password == password);
    }
}