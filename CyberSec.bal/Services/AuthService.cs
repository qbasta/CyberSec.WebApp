using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Context;
using CyberSec.dal.Entities;
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
        Console.WriteLine($"🔍 Sprawdzanie użytkownika: '{username}' z hasłem: '{password}'");
        
        var allUsers = await _dbContext.Users.ToListAsync();
        Console.WriteLine($"📊 Liczba użytkowników w bazie: {allUsers.Count}");
        
        foreach (var user in allUsers)
        {
            Console.WriteLine($"👤 Użytkownik w bazie: '{user.Username}' / '{user.Password}'");
        }

        var result = await _dbContext.Users
            .AnyAsync(u => u.Username == username && u.Password == password);
            
        Console.WriteLine($"🎯 Wynik walidacji: {result}");
        return result;
    }

    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        Console.WriteLine($"📝 Rejestracja użytkownika: '{username}' z hasłem: '{password}'");
        
        if (await _dbContext.Users.AnyAsync(u => u.Username == username))
        {
            Console.WriteLine($"❌ Użytkownik '{username}' już istnieje");
            return false;
        }

        var newUser = new User 
        { 
            Id = Guid.NewGuid().ToString(),
            Username = username, 
            Password = password 
        };
        
        Console.WriteLine($"➕ Dodawanie użytkownika: ID={newUser.Id}, Username='{newUser.Username}', Password='{newUser.Password}'");
        
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();
        
        Console.WriteLine($"✅ Użytkownik '{username}' został zarejestrowany");
        return true;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        Console.WriteLine($"🔍 Szukanie użytkownika: '{username}'");
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);
            
        Console.WriteLine($"👤 Znaleziony użytkownik: {(user != null ? user.Username : "null")}");
        return user;
    }
}