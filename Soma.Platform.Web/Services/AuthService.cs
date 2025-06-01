using Microsoft.AspNetCore.Components.Authorization;
using Soma.Platform.Core.DTOs;
using System.Security.Claims;
using System.Text.Json;

namespace Soma.Platform.Web.Services;

public interface IAuthService
{
    Task<AuthenticationResult> LoginAsync(LoginDto loginDto);
    Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto);
    Task LogoutAsync();
    Task<UserProfile?> GetProfileAsync();
    bool IsAuthenticated { get; }
    string? CurrentToken { get; }
}

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(
        IApiService apiService, 
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _apiService = apiService;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(CurrentToken);
    public string? CurrentToken { get; private set; }

    public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
    {
        Console.WriteLine($"AuthService.LoginAsync called for: {loginDto.EmailOrPhone}");
        var response = await _apiService.PostAsync<LoginResponse>("api/auth/login", loginDto);
        Console.WriteLine($"API response - Success: {response.Success}, Error: {response.ErrorMessage}");
        
        if (response.Success && response.Data != null)
        {
            Console.WriteLine($"API response data - Token: {(string.IsNullOrEmpty(response.Data.Token) ? "null" : "present")}, RequiresTwoFactor: {response.Data.RequiresTwoFactor}, RequiresEmailVerification: {response.Data.RequiresEmailVerification}");
            
            if (response.Data.RequiresTwoFactor)
            {
                return new AuthenticationResult 
                { 
                    Success = true, 
                    RequiresTwoFactor = true,
                    UserId = response.Data.UserId
                };
            }
            
            if (response.Data.RequiresEmailVerification)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    ErrorMessage = response.Data.Message ?? "Please verify your email before logging in.",
                    RequiresEmailVerification = true
                };
            }

            if (!string.IsNullOrEmpty(response.Data.Token))
            {
                Console.WriteLine("Saving token and updating authentication state");
                await SaveTokenAsync(response.Data.Token);
                return new AuthenticationResult { Success = true };
            }
        }

        Console.WriteLine($"Login failed - returning error: {response.ErrorMessage ?? "Login failed"}");
        return new AuthenticationResult 
        { 
            Success = false, 
            ErrorMessage = response.ErrorMessage ?? "Login failed" 
        };
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _apiService.PostAsync<RegisterResponse>("api/auth/register", registerDto);
        
        if (response.Success)
        {
            return new AuthenticationResult 
            { 
                Success = true, 
                RequiresEmailVerification = true 
            };
        }

        return new AuthenticationResult 
        { 
            Success = false, 
            ErrorMessage = response.ErrorMessage ?? "Registration failed" 
        };
    }

    public async Task LogoutAsync()
    {
        await ClearTokenAsync();
    }

    public async Task<UserProfile?> GetProfileAsync()
    {
        if (!IsAuthenticated)
            return null;

        var response = await _apiService.GetAsync<UserProfile>("api/auth/profile");
        return response.Success ? response.Data : null;
    }

    private async Task SaveTokenAsync(string token)
    {
        Console.WriteLine($"SaveTokenAsync called with token length: {token?.Length ?? 0}");
        CurrentToken = token;
        await _localStorage.SetItemAsync("authToken", token);
        _apiService.SetAuthToken(token);
        
        Console.WriteLine("Token saved to localStorage and API service");
        
        // Notify authentication state has changed
        if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            Console.WriteLine("Notifying authentication state provider");
            await customProvider.NotifyUserAuthentication(token);
        }
        else
        {
            Console.WriteLine("Warning: Authentication state provider is not CustomAuthenticationStateProvider");
        }
    }

    private async Task ClearTokenAsync()
    {
        CurrentToken = null;
        await _localStorage.RemoveItemAsync("authToken");
        _apiService.ClearAuthToken();
        
        // Notify authentication state has changed
        if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine("AuthService.InitializeAsync called");
        var token = await _localStorage.GetItemAsync<string>("authToken");
        Console.WriteLine($"Retrieved token from storage: {(string.IsNullOrEmpty(token) ? "null/empty" : "present")}");
        if (!string.IsNullOrEmpty(token))
        {
            CurrentToken = token;
            _apiService.SetAuthToken(token);
            Console.WriteLine("Token set in API service");
        }
    }
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool RequiresEmailVerification { get; set; }
    public string? UserId { get; set; }
}

public class LoginResponse
{
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool RequiresEmailVerification { get; set; }
}

public class RegisterResponse
{
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public bool RequiresEmailVerification { get; set; }
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

// Simplified local storage interface
public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

// Session-based implementation for Blazor Server
public class LocalStorageService : ILocalStorageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<T?> GetItemAsync<T>(string key)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            Console.WriteLine("Warning: Session is null in LocalStorageService.GetItemAsync");
            return Task.FromResult<T?>(default);
        }

        var value = session.GetString(key);
        Console.WriteLine($"LocalStorageService.GetItemAsync - Key: {key}, Value: {(string.IsNullOrEmpty(value) ? "null/empty" : "present")}");
        
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult<T?>(default);
        }
        
        try
        {
            return Task.FromResult(JsonSerializer.Deserialize<T>(value));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing from session: {ex.Message}");
            return Task.FromResult<T?>(default);
        }
    }

    public Task SetItemAsync<T>(string key, T value)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            Console.WriteLine("Warning: Session is null in LocalStorageService.SetItemAsync");
            return Task.CompletedTask;
        }

        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            session.SetString(key, serializedValue);
            Console.WriteLine($"LocalStorageService.SetItemAsync - Key: {key}, Value stored successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serializing to session: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            Console.WriteLine("Warning: Session is null in LocalStorageService.RemoveItemAsync");
            return Task.CompletedTask;
        }

        session.Remove(key);
        Console.WriteLine($"LocalStorageService.RemoveItemAsync - Key: {key} removed");
        return Task.CompletedTask;
    }
}