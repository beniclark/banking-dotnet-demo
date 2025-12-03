using FluentAssertions;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class BankingServiceTests
{
    private readonly BankingService _sut;
    
    // Known demo customer IDs
    private readonly Guid _sarahId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _michaelId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _emilyId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    
    // Known demo account IDs
    private readonly Guid _sarahCheckingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private readonly Guid _sarahSavingsId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public BankingServiceTests()
    {
        _sut = new BankingService();
    }

    #region Customer Operations Tests

    [Fact]
    public async Task GetCustomerByIdAsync_WithValidId_ReturnsCustomer()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Sarah");
        result.LastName.Should().Be("Johnson");
        result.Email.Should().Be("sarah.johnson@email.com");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllDemoCustomers()
    {
        // Act
        var result = await _sut.GetAllCustomersAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.FirstName == "Sarah");
        result.Should().Contain(c => c.FirstName == "Michael");
        result.Should().Contain(c => c.FirstName == "Emily");
    }

    [Fact]
    public async Task GetCustomerProfileAsync_WithValidId_ReturnsProfileWithAccounts()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Sarah Johnson");
        result.Accounts.Should().HaveCount(2);
        result.Accounts.Should().Contain(a => a.AccountType == "Checking");
        result.Accounts.Should().Contain(a => a.AccountType == "Savings");
    }

    [Fact]
    public async Task GetCustomerProfileAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Account Operations Tests

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
    public async Task GetAccountsByCustomerIdAsync_ReturnsCustomerAccounts()
    {
        // Act
        var result = await _sut.GetAccountsByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.CustomerId == _sarahId);
    }

    [Fact]
    public async Task GetAccountSummariesByCustomerIdAsync_ReturnsMaskedAccountNumbers()
    {
        // Act
        var result = await _sut.GetAccountSummariesByCustomerIdAsync(_sarahId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.AccountNumber.StartsWith("****"));
    }

    #endregion

    #region Transaction Operations Tests

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ReturnsTransactionsOrderedByDate()
    {
        // Act
        var result = await _sut.GetTransactionsByAccountIdAsync(_sarahCheckingId);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeInDescendingOrder(t => t.TransactionDate);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_RespectsCountLimit()
    {
        // Act
        var result = await _sut.GetTransactionsByAccountIdAsync(_sarahCheckingId, count: 2);

        // Assert
        result.Should().HaveCountLessOrEqualTo(2);
    }

    #endregion

    #region Transfer Tests

    [Fact]
    public async Task TransferFundsAsync_WithValidRequest_TransfersFunds()
    {
        // Arrange
        var fromAccountBefore = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        var toAccountBefore = await _sut.GetAccountByIdAsync(_sarahSavingsId);
        var fromBalanceBefore = fromAccountBefore!.Balance;
        var toBalanceBefore = toAccountBefore!.Balance;
        var transferAmount = 100.00m;

        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = transferAmount,
            Description = "Test transfer"
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Transfer completed successfully");
        result.ReferenceNumber.Should().StartWith("TRB");
        result.FromTransaction.Should().NotBeNull();
        result.ToTransaction.Should().NotBeNull();

        var fromAccountAfter = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        var toAccountAfter = await _sut.GetAccountByIdAsync(_sarahSavingsId);

        fromAccountAfter!.Balance.Should().Be(fromBalanceBefore - transferAmount);
        toAccountAfter!.Balance.Should().Be(toBalanceBefore + transferAmount);
    }

    [Fact]
    public async Task TransferFundsAsync_WithInsufficientFunds_ReturnsFailure()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = 1_000_000.00m // More than balance
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Insufficient funds");
    }

    [Fact]
    public async Task TransferFundsAsync_WithZeroAmount_ReturnsFailure()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = 0
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Transfer amount must be greater than zero");
    }

    [Fact]
    public async Task TransferFundsAsync_WithNegativeAmount_ReturnsFailure()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = -50.00m
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Transfer amount must be greater than zero");
    }

    [Fact]
    public async Task TransferFundsAsync_WithInvalidFromAccount_ReturnsFailure()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = Guid.NewGuid(),
            ToAccountId = _sarahSavingsId,
            Amount = 100.00m
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("One or both accounts not found");
    }

    [Fact]
    public async Task TransferFundsAsync_WithInvalidToAccount_ReturnsFailure()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = Guid.NewGuid(),
            Amount = 100.00m
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("One or both accounts not found");
    }

    #endregion

    #region Deposit Tests

    [Fact]
    public async Task DepositAsync_WithValidRequest_DepositsAndCreatesTransaction()
    {
        // Arrange
        var accountBefore = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        var balanceBefore = accountBefore!.Balance;
        var depositAmount = 500.00m;

        var request = new DepositRequest
        {
            AccountId = _sarahCheckingId,
            Amount = depositAmount,
            Description = "Test deposit"
        };

        // Act
        var result = await _sut.DepositAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.TransactionType.Should().Be("Credit");
        result.Category.Should().Be("Deposit");
        result.Amount.Should().Be(depositAmount);
        result.Description.Should().Be("Test deposit");

        var accountAfter = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        accountAfter!.Balance.Should().Be(balanceBefore + depositAmount);
    }

    [Fact]
    public async Task DepositAsync_WithDefaultDescription_UsesCashDeposit()
    {
        // Arrange
        var request = new DepositRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 100.00m
        };

        // Act
        var result = await _sut.DepositAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("Cash Deposit");
    }

    [Fact]
    public async Task DepositAsync_WithInvalidAccount_ReturnsNull()
    {
        // Arrange
        var request = new DepositRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m
        };

        // Act
        var result = await _sut.DepositAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DepositAsync_WithZeroAmount_ReturnsNull()
    {
        // Arrange
        var request = new DepositRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 0
        };

        // Act
        var result = await _sut.DepositAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DepositAsync_WithNegativeAmount_ReturnsNull()
    {
        // Arrange
        var request = new DepositRequest
        {
            AccountId = _sarahCheckingId,
            Amount = -100.00m
        };

        // Act
        var result = await _sut.DepositAsync(request);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Withdrawal Tests

    [Fact]
    public async Task WithdrawAsync_WithValidRequest_WithdrawsAndCreatesTransaction()
    {
        // Arrange
        var accountBefore = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        var balanceBefore = accountBefore!.Balance;
        var withdrawAmount = 100.00m;

        var request = new WithdrawalRequest
        {
            AccountId = _sarahCheckingId,
            Amount = withdrawAmount,
            Description = "Test withdrawal"
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.TransactionType.Should().Be("Debit");
        result.Category.Should().Be("Withdrawal");
        result.Amount.Should().Be(withdrawAmount);

        var accountAfter = await _sut.GetAccountByIdAsync(_sarahCheckingId);
        accountAfter!.Balance.Should().Be(balanceBefore - withdrawAmount);
    }

    [Fact]
    public async Task WithdrawAsync_WithInsufficientFunds_ReturnsNull()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 1_000_000.00m
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WithdrawAsync_WithInvalidAccount_ReturnsNull()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WithdrawAsync_WithZeroAmount_ReturnsNull()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 0
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
