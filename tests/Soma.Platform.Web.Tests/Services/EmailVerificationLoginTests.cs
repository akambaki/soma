using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Soma.Platform.Core.DTOs;
using Soma.Platform.Web.Services;

namespace Soma.Platform.Web.Tests.Services;

public class EmailVerificationLoginTests
{
    [Fact]
    public async Task LoginAsync_WithUnverifiedEmail_ShouldReturnCorrectErrorMessage()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
        var mockLocalStorage = new Mock<ILocalStorageService>();

        var authService = new AuthService(
            mockApiService.Object,
            mockAuthStateProvider.Object,
            mockLocalStorage.Object);

        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "Password123!",
            RememberMe = false
        };

        // Simulate the backend response when email verification is required
        var loginResponse = new LoginResponse
        {
            Message = "Email not verified. Please check your email for verification link.",
            RequiresEmailVerification = true,
            UserId = "user-123"
        };

        var apiResponse = new ApiResponse<LoginResponse>
        {
            Success = true,
            Data = loginResponse
        };

        mockApiService.Setup(x => x.PostAsync<LoginResponse>("api/auth/login", loginDto))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await authService.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeFalse();
        result.RequiresEmailVerification.Should().BeTrue();
        result.ErrorMessage.Should().Be("Email not verified. Please check your email for verification link.");
        result.ErrorMessage.Should().NotBe("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_WithBackendError_ShouldReturnApiErrorMessage()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
        var mockLocalStorage = new Mock<ILocalStorageService>();

        var authService = new AuthService(
            mockApiService.Object,
            mockAuthStateProvider.Object,
            mockLocalStorage.Object);

        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "WrongPassword",
            RememberMe = false
        };

        // Simulate API service returning an error (e.g., 401 Unauthorized)
        var apiResponse = new ApiResponse<LoginResponse>
        {
            Success = false,
            ErrorMessage = "Invalid credentials"
        };

        mockApiService.Setup(x => x.PostAsync<LoginResponse>("api/auth/login", loginDto))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await authService.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid credentials");
    }
}