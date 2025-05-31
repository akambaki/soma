using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Soma.Platform.Core.DTOs;

namespace Soma.Platform.Core.Tests;

public class DtoValidationTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void RegisterDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = true,
            PhoneNumber = "+1234567890"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void RegisterDto_WithInvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "invalid-email",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().Contain(r => r.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void RegisterDto_WithWeakPassword_ShouldFailValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "weak",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().Contain(r => r.ErrorMessage!.Contains("Password must be at least 8 characters"));
    }

    [Fact]
    public void RegisterDto_WithoutAcceptingTerms_ShouldFailValidation()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = false
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().Contain(r => r.ErrorMessage == "You must accept the terms and conditions");
    }

    [Fact]
    public void LoginDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var dto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "Password123!",
            RememberMe = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void LoginDto_WithEmptyEmailOrPhone_ShouldFailValidation()
    {
        // Arrange
        var dto = new LoginDto
        {
            EmailOrPhone = "",
            Password = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().Contain(r => r.ErrorMessage == "Email or phone number is required");
    }

    [Fact]
    public void TwoFactorDto_WithValidCode_ShouldPassValidation()
    {
        // Arrange
        var dto = new TwoFactorDto
        {
            UserId = "user123",
            Code = "123456"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void TwoFactorDto_WithInvalidCode_ShouldFailValidation()
    {
        // Arrange
        var dto = new TwoFactorDto
        {
            UserId = "user123",
            Code = "12345" // Less than 6 digits
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().Contain(r => r.ErrorMessage == "Code must be 6 digits");
    }

    [Fact]
    public void ForgotPasswordDto_WithValidEmail_ShouldPassValidation()
    {
        // Arrange
        var dto = new ForgotPasswordDto
        {
            Email = "test@example.com"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void ResetPasswordDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var dto = new ResetPasswordDto
        {
            Token = "valid-token",
            Email = "test@example.com",
            NewPassword = "NewPassword123!"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }
}