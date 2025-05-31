using FluentAssertions;
using Soma.Platform.Core.Models;

namespace Soma.Platform.Core.Tests.Models;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.Is2FARequired.Should().BeTrue();
        user.EmailVerified.Should().BeFalse();
        user.PhoneNumberVerified.Should().BeFalse();
        user.LastLoginAt.Should().BeNull();
        user.VerificationToken.Should().BeNull();
        user.VerificationTokenExpiry.Should().BeNull();
        user.GoogleId.Should().BeNull();
        user.AppleId.Should().BeNull();
        user.ProfileImageUrl.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_WhenSetProperties_ShouldRetainValues()
    {
        // Arrange
        var user = new ApplicationUser();
        var testDate = DateTime.UtcNow.AddMinutes(-5);

        // Act
        user.FirstName = "John";
        user.LastName = "Doe";
        user.Email = "john.doe@example.com";
        user.PhoneNumber = "+1234567890";
        user.LastLoginAt = testDate;
        user.EmailVerified = true;
        user.PhoneNumberVerified = true;
        user.Is2FARequired = false;
        user.VerificationToken = "test-token";
        user.VerificationTokenExpiry = testDate.AddHours(1);
        user.GoogleId = "google-123";
        user.AppleId = "apple-456";
        user.ProfileImageUrl = "https://example.com/profile.jpg";

        // Assert
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john.doe@example.com");
        user.PhoneNumber.Should().Be("+1234567890");
        user.LastLoginAt.Should().Be(testDate);
        user.EmailVerified.Should().BeTrue();
        user.PhoneNumberVerified.Should().BeTrue();
        user.Is2FARequired.Should().BeFalse();
        user.VerificationToken.Should().Be("test-token");
        user.VerificationTokenExpiry.Should().Be(testDate.AddHours(1));
        user.GoogleId.Should().Be("google-123");
        user.AppleId.Should().Be("apple-456");
        user.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("John", "")]
    [InlineData("", "Doe")]
    [InlineData("John", "Doe")]
    public void ApplicationUser_WithDifferentNameCombinations_ShouldAcceptValues(string firstName, string lastName)
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.FirstName = firstName;
        user.LastName = lastName;

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
    }
}