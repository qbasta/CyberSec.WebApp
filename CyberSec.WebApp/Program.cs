using CyberSec.bal.Services;
using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Context;
using CyberSec.WebApp.Components;
using CyberSec.WebApp.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region Database Configuration
builder.Services.AddDbContext<CyberSecDbContext>(options =>
{
    options.UseSqlite("Data Source=cybersec.db", 
        b => b.MigrationsAssembly("CyberSec.WebApp"));
});
#endregion

#region Authentication & Authorization
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
#endregion

#region Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IUserService, UserService>();
#endregion

var app = builder.Build();

#region Database Migration on Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CyberSecDbContext>();
    
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Baza danych została zmigrowana pomyślnie.");
        
        // Usuń użytkownika z pustymi danymi
        var emptyUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "");
        if (emptyUser != null)
        {
            dbContext.Users.Remove(emptyUser);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("🗑️ Usunięto użytkownika z pustymi danymi.");
        }
        
        // Dodaj testowego użytkownika
        if (!await dbContext.Users.AnyAsync(u => u.Username == "test"))
        {
            var testUser = new CyberSec.dal.Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "test",
                Password = "test123"
            };
            
            dbContext.Users.Add(testUser);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("✅ Dodano testowego użytkownika: test/test123");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Błąd migracji bazy danych: {ex.Message}");
    }
}
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();