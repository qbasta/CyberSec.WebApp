using System.Security.Claims;
using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Entities;
using CyberSec.dal.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace CyberSec.WebApp.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private ClaimsPrincipal _anonymous => new ClaimsPrincipal(new ClaimsIdentity());
    
    private LoginModel? _currentUser = null;

    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("🔵 Wywołano `GetAuthenticationStateAsync()`");

        if (_currentUser != null)
        {
            Console.WriteLine($"✅ Sesja odnaleziona: {_currentUser.Username}");

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _currentUser.Username)
            }, "CustomAuth");

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        Console.WriteLine("⚠️ Brak aktywnej sesji.");
        return Task.FromResult(new AuthenticationState(_anonymous));
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        Console.WriteLine($"🔵 Próba logowania: {username}");

        var isValid = await _authService.ValidateUserAsync(username, password);
        Console.WriteLine($"🔍 Walidacja użytkownika {username}: {isValid}");

        if (isValid)
        {
            Console.WriteLine($"✅ Logowanie udane: {username}");

            _currentUser = new LoginModel { Username = username, Password = password };
            Console.WriteLine("🔵 Sesja została zapisana.");

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "CustomAuth");

            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));

            return true;
        }

        Console.WriteLine($"❌ Błędne dane logowania dla: {username}");
        return false;
    }

    public Task LogoutAsync()
    {
        Console.WriteLine("🔵 Wylogowanie użytkownika.");
        _currentUser = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        return Task.CompletedTask;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        var username = authState.User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
            return null;
            
        return await _authService.GetUserByUsernameAsync(username);
    }
}