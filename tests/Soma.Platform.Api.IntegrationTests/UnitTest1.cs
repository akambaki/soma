using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Soma.Platform.Core.Data;
using Soma.Platform.Core.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Soma.Platform.Api.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GET_Health_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task POST_Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Password123!",
            FirstName = "Integration",
            LastName = "Test",
            AcceptTerms = true,
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("message").GetString().Should().Contain("Registration successful");
        result.GetProperty("requiresEmailVerification").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task POST_Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var registerDto = new RegisterDto
        {
            Email = email,
            Password = "Password123!",
            FirstName = "Integration",
            LastName = "Test",
            AcceptTerms = true
        };

        // First registration
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Act - Try to register again with same email
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            EmailOrPhone = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_Register_Then_Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var email = $"logintest{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        
        var registerDto = new RegisterDto
        {
            Email = email,
            Password = password,
            FirstName = "Login",
            LastName = "Test",
            AcceptTerms = true
        };

        // First, register the user
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Manually confirm the user's email (simulate email verification)
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        user.Should().NotBeNull();
        user!.EmailConfirmed = true;
        await dbContext.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            EmailOrPhone = email,
            Password = password
        };

        // Act - Try to login
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await loginResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task POST_Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "invalid-email", // Invalid email format
            Password = "weak", // Weak password
            FirstName = "", // Empty first name
            LastName = "", // Empty last name
            AcceptTerms = false // Terms not accepted
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_Profile_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}