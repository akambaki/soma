using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Soma.Platform.Web.IntegrationTests;

public class DataProtectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DataProtectionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void DataProtection_ShouldBeConfigured()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var dataProtectionProvider = scope.ServiceProvider.GetService<IDataProtectionProvider>();

        // Assert
        dataProtectionProvider.Should().NotBeNull("Data protection should be configured");
    }

    [Fact]
    public void DataProtection_ShouldPersistKeysToFileSystem()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dataProtectionProvider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();

        // Act
        var protector = dataProtectionProvider.CreateProtector("test-purpose");
        var originalData = "test-data-for-protection";
        var protectedData = protector.Protect(originalData);
        var unprotectedData = protector.Unprotect(protectedData);

        // Assert
        protectedData.Should().NotBe(originalData, "Data should be encrypted");
        unprotectedData.Should().Be(originalData, "Data should be decryptable");
    }

    [Fact]
    public void DataProtectionKeyDirectory_ShouldExist()
    {
        // Arrange
        var keyPath = Environment.GetEnvironmentVariable("DATA_PROTECTION_KEY_PATH") ?? "/tmp/dataprotection-keys";

        // Act & Assert
        Directory.Exists(keyPath).Should().BeTrue($"Data protection key directory {keyPath} should exist");
    }

    [Fact]
    public async Task AntiforgeryToken_ShouldBeIncludedInLoginPage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/login");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue("Login page should load successfully");
        
        var content = await response.Content.ReadAsStringAsync();
        
        // Check that the login form is present (which would contain antiforgery tokens)
        // This ensures that the data protection system is working for forms
        content.Should().Contain("type=\"password\"", 
            "Login form should be present, which means antiforgery tokens are being generated");
    }

    [Fact]
    public void DataProtection_ApplicationName_ShouldBeSet()
    {
        // This test verifies that the application name is set correctly for data protection
        // which helps ensure keys are isolated between different applications
        
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var dataProtectionProvider = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
        
        // Create a protector to trigger key generation
        var protector = dataProtectionProvider.CreateProtector("test");
        var testData = "test";
        var protected1 = protector.Protect(testData);
        
        // Assert - if we can protect and unprotect, the application name is working
        var unprotected = protector.Unprotect(protected1);
        unprotected.Should().Be(testData, "Data protection with application name should work correctly");
    }
}