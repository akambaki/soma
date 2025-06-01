using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Soma.Platform.Web.Components;
using Soma.Platform.Web.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Data Protection for containerized environment
var dataProtectionKeyPath = Environment.GetEnvironmentVariable("DATA_PROTECTION_KEY_PATH") ?? "/tmp/dataprotection-keys";

// Ensure the key directory exists
Directory.CreateDirectory(dataProtectionKeyPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath))
    .SetApplicationName("Soma.Platform.Web");

// Add logging for data protection
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add HTTP client
builder.Services.AddHttpClient<IApiService, ApiService>();

// Add authentication services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

// Add authorization
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Only use HTTPS redirection in non-containerized environments
if (!app.Environment.IsProduction() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
