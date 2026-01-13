using FluentAssertions;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AuthService _sut;
    private readonly TestDatabaseFactory _dbFactory;

    public AuthServiceTests()
    {
        _dbFactory = new TestDatabaseFactory();
        _sut = new AuthService(_dbFactory.DbContext);
    }

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "sarah.johnson",
            Password = "password123"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Login successful");
        result.CustomerId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        result.CustomerName.Should().Be("Sarah Johnson");
    }

    [Fact]
    public async Task LoginAsync_WithEmail_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "michael.chen@email.com",
            Password = "password123"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CustomerId.Should().Be(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        result.CustomerName.Should().Be("Michael Chen");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent.user",
            Password = "password123"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid username or password");
        result.CustomerId.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "sarah.johnson",
            Password = "wrongpassword"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid username or password");
        result.CustomerId.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithEmptyUsername_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "",
            Password = "password123"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Username and password are required");
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "sarah.johnson",
            Password = ""
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Username and password are required");
    }

    #endregion

    #region GetUserByUsernameAsync Tests

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
    {
        // Act
        var result = await _sut.GetUserByUsernameAsync("sarah.johnson");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("sarah.johnson");
        result.Email.Should().Be("sarah.johnson@email.com");
        result.CustomerId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithEmail_ReturnsUser()
    {
        // Act
        var result = await _sut.GetUserByUsernameAsync("emily.rodriguez@email.com");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("emily.rodriguez");
        result.Email.Should().Be("emily.rodriguez@email.com");
        result.CustomerId.Should().Be(Guid.Parse("33333333-3333-3333-3333-333333333333"));
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ReturnsNull()
    {
        // Act
        var result = await _sut.GetUserByUsernameAsync("nonexistent.user");

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
