using FluentAssertions;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AuthService _sut;
    private readonly CustomerService _customerService;
    private readonly TestDatabaseFactory _dbFactory;

    public AuthServiceTests()
    {
        _dbFactory = new TestDatabaseFactory();
        _customerService = new CustomerService(_dbFactory.DbContext);
        _sut = new AuthService(_dbFactory.DbContext, _customerService);
    }

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidEmail_ReturnsSuccessWithCustomerProfile()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "sarah.johnson@email.com"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Login successful");
        result.Customer.Should().NotBeNull();
        result.Customer!.FullName.Should().Be("Sarah Johnson");
        result.Customer.Email.Should().Be("sarah.johnson@email.com");
        result.Customer.Accounts.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithValidEmailDifferentCase_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "SARAH.JOHNSON@EMAIL.COM"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Customer.Should().NotBeNull();
        result.Customer!.Email.Should().Be("sarah.johnson@email.com");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@email.com"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("No account found with this email address.");
        result.Customer.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithEmptyEmail_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = ""
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Please provide a valid email address.");
        result.Customer.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmailFormat_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "not-an-email"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Please provide a valid email address.");
        result.Customer.Should().BeNull();
    }

    [Theory]
    [InlineData("sarah.johnson@email.com")]
    [InlineData("michael.chen@email.com")]
    [InlineData("emily.rodriguez@email.com")]
    public async Task LoginAsync_WithAllDemoUsers_ReturnsSuccess(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Customer.Should().NotBeNull();
        result.Customer!.Email.Should().Be(email);
    }

    #endregion
}
