namespace Soma.Platform.Core.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationLink);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
    Task Send2FACodeEmailAsync(string email, string code);
}

public interface ISmsService
{
    Task SendVerificationSmsAsync(string phoneNumber, string code);
    Task Send2FACodeSmsAsync(string phoneNumber, string code);
}

public interface ITwoFactorService
{
    Task<string> GenerateQrCodeAsync(string userEmail, string secretKey);
    Task<bool> ValidateCodeAsync(string secretKey, string code);
    string GenerateSecretKey();
}