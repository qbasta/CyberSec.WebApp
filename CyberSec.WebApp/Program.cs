using CyberSec.bal.Services;
using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Context;
using CyberSec.dal.Entities;
using CyberSec.WebApp.Components;
using CyberSec.WebApp.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Session providers
builder.Services.AddScoped<ProtectedSessionStorage>(); 
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();


// Services
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<CyberSecDbContext>(options =>
{
    options.UseSqlite("Data Source=cybersec.db");
});

var app = builder.Build();

#region BasicUserOnLaunch
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<CyberSecDbContext>();
//
//     if (!dbContext.Users.Any())
//     {
//         dbContext.Users.Add(new User { Username = "login", Password = "haslo" });
//         dbContext.SaveChanges();
//     }
// }
#endregion


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();