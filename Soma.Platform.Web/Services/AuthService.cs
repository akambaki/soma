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
        var response = await _apiService.PostAsync<LoginResponse>("api/auth/login", loginDto);
        
        if (response.Success && response.Data != null)
        {
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
                await SaveTokenAsync(response.Data.Token);
                return new AuthenticationResult { Success = true };
            }
        }

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
        CurrentToken = token;
        await _localStorage.SetItemAsync("authToken", token);
        _apiService.SetAuthToken(token);
        
        // Notify authentication state has changed
        if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            await customProvider.NotifyUserAuthentication(token);
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
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            CurrentToken = token;
            _apiService.SetAuthToken(token);
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

// In-memory implementation for Blazor Server to avoid session timing issues
public class LocalStorageService : ILocalStorageService
{
    private readonly Dictionary<string, string> _storage = new();
    private readonly object _lock = new();

    public Task<T?> GetItemAsync<T>(string key)
    {
        lock (_lock)
        {
            if (!_storage.TryGetValue(key, out var value) || string.IsNullOrEmpty(value))
            {
                return Task.FromResult<T?>(default);
            }
            
            try
            {
                return Task.FromResult(JsonSerializer.Deserialize<T>(value));
            }
            catch (Exception)
            {
                return Task.FromResult<T?>(default);
            }
        }
    }

    public Task SetItemAsync<T>(string key, T value)
    {
        lock (_lock)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                _storage[key] = serializedValue;
            }
            catch (Exception)
            {
                // Silently handle serialization errors
            }
        }
        
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        lock (_lock)
        {
            _storage.Remove(key);
        }
        return Task.CompletedTask;
    }
}