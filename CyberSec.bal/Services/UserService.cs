using CyberSec.dal.Context;
using CyberSec.dal.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string userId);
}

public class UserService : IUserService
{
    private readonly CyberSecDbContext _dbContext;

    public UserService(CyberSecDbContext context)
    {
        _dbContext = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }
}