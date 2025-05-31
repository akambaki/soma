using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Soma.Platform.Core.DTOs;
using Soma.Platform.Web.Services;

namespace Soma.Platform.Web.Tests.Services;

public class AuthServiceTests : TestContext
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly Mock<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider> _mockAuthStateProvider;
    private readonly Mock<ILocalStorageService> _mockLocalStorage;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockApiService = new Mock<IApiService>();
        _mockAuthStateProvider = new Mock<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>();
        _mockLocalStorage = new Mock<ILocalStorageService>();

        _authService = new AuthService(
            _mockApiService.Object,
            _mockAuthStateProvider.Object,
            _mockLocalStorage.Object);
    }

    [Fact]
    public void IsAuthenticated_WhenNoToken_ShouldReturnFalse()
    {
        // Act & Assert
        _authService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void CurrentToken_WhenNoToken_ShouldReturnNull()
    {
        // Act & Assert
        _authService.CurrentToken.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "Password123!",
            RememberMe = false
        };

        var loginResponse = new LoginResponse
        {
            Token = "test-token",
            UserId = "user-id",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var apiResponse = new ApiResponse<LoginResponse>
        {
            Success = true,
            Data = loginResponse
        };

        _mockApiService.Setup(x => x.PostAsync<LoginResponse>("api/auth/login", loginDto))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeTrue();
        result.RequiresTwoFactor.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = true
        };

        var registerResponse = new RegisterResponse
        {
            Message = "Registration successful",
            UserId = "user-id",
            RequiresEmailVerification = true
        };

        var apiResponse = new ApiResponse<RegisterResponse>
        {
            Success = true,
            Data = registerResponse
        };

        _mockApiService.Setup(x => x.PostAsync<RegisterResponse>("api/auth/register", registerDto))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Success.Should().BeTrue();
    }
}