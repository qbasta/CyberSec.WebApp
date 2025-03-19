using System.Security.Claims;
using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CyberSec.WebApp.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous => new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(IAuthService authService, ProtectedSessionStorage sessionStorage)
    {
        _authService = authService;
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userResult = await _sessionStorage.GetAsync<LoginModel>("user");
            if (userResult.Success && userResult.Value is not null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userResult.Value.Username)
                }, "CustomAuth");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }

        return new AuthenticationState(_anonymous);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (await _authService.ValidateUserAsync(username, password))
        {
            var userModel = new LoginModel { Username = username, Password = password };
            await _sessionStorage.SetAsync("user", userModel);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "CustomAuth");

            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync("user");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
