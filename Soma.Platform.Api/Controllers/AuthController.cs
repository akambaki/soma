using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Soma.Platform.Core.DTOs;
using Soma.Platform.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Soma.Platform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = string.IsNullOrEmpty(model.PhoneNumber) ? true : false,
                Is2FARequired = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate email verification token
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                _logger.LogInformation("User {Email} registered successfully", model.Email);
                
                return Ok(new 
                { 
                    message = "Registration successful. Please check your email for verification link.",
                    userId = user.Id,
                    requiresEmailVerification = true
                });
            }

            return BadRequest(new { message = "Registration failed", errors = result.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = null;

            // Check if login is email or phone
            if (model.EmailOrPhone.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(model.EmailOrPhone);
            }
            else
            {
                // Find by phone number
                var users = _userManager.Users.Where(u => u.PhoneNumber == model.EmailOrPhone);
                user = users.FirstOrDefault();
            }

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for {EmailOrPhone}", model.EmailOrPhone);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User found: {Email}, EmailConfirmed: {EmailConfirmed}", user.Email, user.EmailConfirmed);

            // First check if the password is correct
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            
            if (!passwordValid)
            {
                // Increment failed access count for security
                await _userManager.AccessFailedAsync(user);
                _logger.LogWarning("Invalid password for user {Email}", user.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check if account is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Account locked out for user {Email}", user.Email);
                return BadRequest(new { message = "Account locked due to multiple failed attempts" });
            }

            // Reset access failed count on successful password verification
            await _userManager.ResetAccessFailedCountAsync(user);

            // Now check email confirmation
            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Login blocked due to unverified email for user {Email}", user.Email);
                return Ok(new 
                { 
                    message = "Email not verified. Please check your email for verification link.",
                    requiresEmailVerification = true,
                    userId = user.Id
                });
            }

            // Check 2FA requirement
            if (user.Is2FARequired && user.TwoFactorEnabled)
            {
                return Ok(new 
                { 
                    message = "Two-factor authentication required",
                    requiresTwoFactor = true,
                    userId = user.Id
                });
            }

            // Successful login - generate token
            var token = await GenerateJwtToken(user);
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Ok(new 
            { 
                token = token,
                userId = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email verified successfully" });
            }

            return BadRequest(new { message = "Email verification failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification");
            return StatusCode(500, new { message = "An error occurred during email verification" });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Ok(new { message = "If an account with this email exists, you will receive a password reset link." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // In a real application, send this via email
            _logger.LogInformation("Password reset token for {Email}: {Token}", model.Email, token);
            
            return Ok(new { message = "If an account with this email exists, you will receive a password reset link." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password reset successful" });
            }

            return BadRequest(new { message = "Password reset failed", errors = result.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(500, new { message = "An error occurred during password reset" });
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new 
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                phoneNumber = user.PhoneNumber,
                emailVerified = user.EmailConfirmed,
                phoneVerified = user.PhoneNumberConfirmed,
                twoFactorEnabled = user.TwoFactorEnabled,
                createdAt = user.CreatedAt,
                lastLoginAt = user.LastLoginAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2FA([FromBody] TwoFactorDto model)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            // For now, use a simple 2FA verification (in production, integrate with TOTP)
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, "Email", model.Code);

            if (isValid)
            {
                var token = await GenerateJwtToken(user);
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return Ok(new 
                { 
                    token = token,
                    userId = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email
                });
            }

            return BadRequest(new { message = "Invalid verification code" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during 2FA verification");
            return StatusCode(500, new { message = "An error occurred during verification" });
        }
    }

    [HttpPost("setup-2fa")]
    [Authorize]
    public async Task<IActionResult> Setup2FA()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            
            // Generate setup key (in production, use proper TOTP setup)
            var setupKey = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            
            return Ok(new 
            { 
                message = "Two-factor authentication enabled",
                recoveryCodes = setupKey
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up 2FA");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("disable-2fa")]
    [Authorize]
    public async Task<IActionResult> Disable2FA([FromBody] DisableTwoFactorDto model)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Verify password before disabling 2FA
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return BadRequest(new { message = "Invalid password" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            
            return Ok(new { message = "Two-factor authentication disabled" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("firstName", user.FirstName ?? ""),
            new("lastName", user.LastName ?? "")
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "SomaSecretKeyForJWTTokenGeneration2024!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "Soma.Platform",
            audience: _configuration["Jwt:Audience"] ?? "Soma.Platform.Users",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class VerifyEmailDto
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class DisableTwoFactorDto
{
    public string Password { get; set; } = string.Empty;
}