using Microsoft.AspNetCore.Identity;

namespace Soma.Platform.Core.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool Is2FARequired { get; set; } = true;
    public string? ProfileImageUrl { get; set; }
    
    // Additional authentication fields
    public bool EmailVerified { get; set; }
    public bool PhoneNumberVerified { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? VerificationTokenExpiry { get; set; }
    
    // OAuth integration
    public string? GoogleId { get; set; }
    public string? AppleId { get; set; }
}
