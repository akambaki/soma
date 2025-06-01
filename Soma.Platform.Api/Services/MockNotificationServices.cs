using Soma.Platform.Core.Services;

namespace Soma.Platform.Api.Services;

public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationEmailAsync(string email, string verificationLink)
    {
        _logger.LogInformation("📧 Mock Email: Verification email sent to {Email}", email);
        _logger.LogInformation("📧 Verification Link: {Link}", verificationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        _logger.LogInformation("📧 Mock Email: Password reset email sent to {Email}", email);
        _logger.LogInformation("📧 Reset Link: {Link}", resetLink);
        return Task.CompletedTask;
    }

    public Task Send2FACodeEmailAsync(string email, string code)
    {
        _logger.LogInformation("📧 Mock Email: 2FA code sent to {Email}", email);
        _logger.LogInformation("📧 2FA Code: {Code}", code);
        return Task.CompletedTask;
    }
}

public class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;

    public MockSmsService(ILogger<MockSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationSmsAsync(string phoneNumber, string code)
    {
        _logger.LogInformation("📱 Mock SMS: Verification code sent to {PhoneNumber}", phoneNumber);
        _logger.LogInformation("📱 Verification Code: {Code}", code);
        return Task.CompletedTask;
    }

    public Task Send2FACodeSmsAsync(string phoneNumber, string code)
    {
        _logger.LogInformation("📱 Mock SMS: 2FA code sent to {PhoneNumber}", phoneNumber);
        _logger.LogInformation("📱 2FA Code: {Code}", code);
        return Task.CompletedTask;
    }
}