using FluentAssertions;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class AccountServiceTests : IDisposable
{
    private readonly AccountService _sut;
    private readonly TestDatabaseFactory _dbFactory;

    // Known demo customer IDs
    private readonly Guid _sarahId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _michaelId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    // Known demo account IDs
    private readonly Guid _sarahCheckingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private readonly Guid _sarahSavingsId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public AccountServiceTests()
    {
        _dbFactory = new TestDatabaseFactory();
        _sut = new AccountService(_dbFactory.DbContext);
    }

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    #region GetAccountByIdAsync Tests

    [Fact]
    public async Task GetAccountByIdAsync_WithValidId_ReturnsAccount()
    {
        // Act
        var result = await _sut.GetAccountByIdAsync(_sarahCheckingId);

        // Assert
        result.Should().NotBeNull();
        result!.AccountType.Should().Be("Checking");
        result.AccountName.Should().Be("Primary Checking");
        result.CustomerId.Should().Be(_sarahId);
    }

    [Fact]
    public async Task GetAccountByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetAccountByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAccountByIdAsync_WithSavingsAccount_ReturnsSavingsAccount()
    {
        // Act
        var result = await _sut.GetAccountByIdAsync(_sarahSavingsId);

        // Assert
        result.Should().NotBeNull();
        result!.AccountType.Should().Be("Savings");
        result.AccountName.Should().Be("Emergency Fund");
    }

    [Fact]
    public async Task GetAccountByIdAsync_ReturnsAccountWithCorrectBalance()
    {
        // Act
        var result = await _sut.GetAccountByIdAsync(_sarahCheckingId);

        // Assert
        result.Should().NotBeNull();
        result!.Balance.Should().Be(5432.87m);
        result.AvailableBalance.Should().Be(5432.87m);
    }

    #endregion

    #region GetAccountsByCustomerIdAsync Tests

    [Fact]
    public async Task GetAccountsByCustomerIdAsync_ReturnsCustomerAccounts()
    {
        // Act
        var result = await _sut.GetAccountsByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.CustomerId == _sarahId);
    }

    [Fact]
    public async Task GetAccountsByCustomerIdAsync_WithInvalidCustomerId_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetAccountsByCustomerIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAccountsByCustomerIdAsync_ReturnsBothCheckingAndSavings()
    {
        // Act
        var result = await _sut.GetAccountsByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().Contain(a => a.AccountType == "Checking");
        result.Should().Contain(a => a.AccountType == "Savings");
    }

    [Fact]
    public async Task GetAccountsByCustomerIdAsync_ForMichael_ReturnsTwoAccounts()
    {
        // Act
        var result = await _sut.GetAccountsByCustomerIdAsync(_michaelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.CustomerId == _michaelId);
    }

    #endregion

    #region GetAccountSummariesByCustomerIdAsync Tests

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_ReturnsMaskedAccountNumbers()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.AccountNumber.StartsWith("****"));
    }

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_WithInvalidCustomerId_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_ReturnsCorrectAccountTypes()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().Contain(a => a.AccountType == "Checking");
        result.Should().Contain(a => a.AccountType == "Savings");
    }

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_ReturnsCorrectBalances()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(_sarahId);

        // Assert
        var checking = result.First(a => a.AccountType == "Checking");
        var savings = result.First(a => a.AccountType == "Savings");

        checking.Balance.Should().Be(5432.87m);
        savings.Balance.Should().Be(15750.00m);
    }

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_MasksAccountNumberCorrectly()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().OnlyContain(a => a.AccountNumber.Length == 8); // "****" + last 4 digits
    }

    #endregion
}
