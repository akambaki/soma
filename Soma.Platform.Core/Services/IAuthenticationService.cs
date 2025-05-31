using Soma.Platform.Core.Models;

namespace Soma.Platform.Core.Services;

public interface IAuthenticationService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> LoginWith2FAAsync(string userId, string code);
    Task<AuthResult> OAuthLoginAsync(OAuthRequest request);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> VerifyPhoneAsync(string userId, string code);
    Task<bool> ResendVerificationAsync(string email);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> Enable2FAAsync(string userId);
    Task<bool> Disable2FAAsync(string userId, string password);
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; }
}

public class LoginRequest
{
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class OAuthRequest
{
    public string Provider { get; set; } = string.Empty; // "google", "apple"
    public string ProviderId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool RequiresEmailVerification { get; set; }
    public bool RequiresPhoneVerification { get; set; }
    public string? ErrorMessage { get; set; }
    public ApplicationUser? User { get; set; }
}