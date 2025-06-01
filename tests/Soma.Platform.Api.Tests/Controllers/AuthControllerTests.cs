using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Soma.Platform.Api.Controllers;
using Soma.Platform.Core.DTOs;
using Soma.Platform.Core.Models;

namespace Soma.Platform.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // Setup UserManager mock
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        // Setup SignInManager mock
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Register_WithValidModel_ShouldReturnOk()
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

        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("test-token");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WithExistingUser_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            AcceptTerms = true
        };

        var existingUser = new ApplicationUser { Email = registerDto.Email };
        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badResult = result as BadRequestObjectResult;
        badResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WithInvalidModel_ShouldReturnBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto(); // Invalid model - missing required fields
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WhenUserCreationFails_ShouldReturnBadRequest()
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

        _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var identityErrors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password too short" }
        };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "Password123!",
            RememberMe = false
        };

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@example.com",
            UserName = "test@example.com",
            EmailConfirmed = true
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.EmailOrPhone))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        _mockUserManager.Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(false);

        _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _mockUserManager.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Setup configuration for JWT
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(x => x.Value).Returns("your-very-long-secret-key-that-is-at-least-256-bits");
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("SomaSecretKeyForJWTTokenGeneration2024!");

        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("Soma.Platform");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("Soma.Platform.Users");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "WrongPassword",
            RememberMe = false
        };

        var user = new ApplicationUser
        {
            Email = "test@example.com",
            UserName = "test@example.com"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.EmailOrPhone))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        _mockUserManager.Setup(x => x.AccessFailedAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithUnconfirmedEmail_ShouldReturnOkWithEmailVerificationRequired()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            EmailOrPhone = "test@example.com",
            Password = "Password123!",
            RememberMe = false
        };

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@example.com",
            UserName = "test@example.com",
            EmailConfirmed = false // Email not confirmed
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.EmailOrPhone))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        _mockUserManager.Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(false);

        _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        // Verify the response contains email verification requirement
        var responseValue = okResult.Value;
        var responseType = responseValue!.GetType();
        var requiresEmailVerificationProperty = responseType.GetProperty("requiresEmailVerification");
        requiresEmailVerificationProperty.Should().NotBeNull();
        requiresEmailVerificationProperty!.GetValue(responseValue).Should().Be(true);
    }
}