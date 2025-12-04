using FluentAssertions;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class TransactionServiceTests
{
    private readonly TransactionService _sut;
    private readonly BankingDataStore _dataStore;

    // Known demo account IDs
    private readonly Guid _sarahCheckingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private readonly Guid _sarahSavingsId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public TransactionServiceTests()
    {
        _dataStore = new BankingDataStore();
        _sut = new TransactionService(_dataStore);
    }

    #region GetTransactionsByAccountIdAsync Tests

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

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_WithInvalidAccountId_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetTransactionsByAccountIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ReturnsTransactionsForCorrectAccount()
    {
        // Act
        var result = await _sut.GetTransactionsByAccountIdAsync(_sarahCheckingId);

        // Assert
        result.Should().OnlyContain(t => t.AccountId == _sarahCheckingId);
    }

    #endregion

    #region GetTransactionByIdAsync Tests

    [Fact]
    public async Task GetTransactionByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetTransactionByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region TransferFundsAsync Tests

    [Fact]
    public async Task TransferFundsAsync_WithValidRequest_TransfersFunds()
    {
        // Arrange
        var fromAccountBefore = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        var toAccountBefore = _dataStore.Accounts.First(a => a.Id == _sarahSavingsId);
        var fromBalanceBefore = fromAccountBefore.Balance;
        var toBalanceBefore = toAccountBefore.Balance;
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

        var fromAccountAfter = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        var toAccountAfter = _dataStore.Accounts.First(a => a.Id == _sarahSavingsId);

        fromAccountAfter.Balance.Should().Be(fromBalanceBefore - transferAmount);
        toAccountAfter.Balance.Should().Be(toBalanceBefore + transferAmount);
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

    [Fact]
    public async Task TransferFundsAsync_CreatesDebitTransaction()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = 50.00m
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.FromTransaction.Should().NotBeNull();
        result.FromTransaction!.TransactionType.Should().Be("Debit");
        result.FromTransaction.Category.Should().Be("Transfer");
    }

    [Fact]
    public async Task TransferFundsAsync_CreatesCreditTransaction()
    {
        // Arrange
        var request = new TransferRequest
        {
            FromAccountId = _sarahCheckingId,
            ToAccountId = _sarahSavingsId,
            Amount = 50.00m
        };

        // Act
        var result = await _sut.TransferFundsAsync(request);

        // Assert
        result.ToTransaction.Should().NotBeNull();
        result.ToTransaction!.TransactionType.Should().Be("Credit");
        result.ToTransaction.Category.Should().Be("Transfer");
    }

    #endregion

    #region DepositAsync Tests

    [Fact]
    public async Task DepositAsync_WithValidRequest_DepositsAndCreatesTransaction()
    {
        // Arrange
        var accountBefore = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        var balanceBefore = accountBefore.Balance;
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

        var accountAfter = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        accountAfter.Balance.Should().Be(balanceBefore + depositAmount);
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

    [Fact]
    public async Task DepositAsync_GeneratesReferenceNumber()
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
        result!.ReferenceNumber.Should().StartWith("TRB");
    }

    #endregion

    #region WithdrawAsync Tests

    [Fact]
    public async Task WithdrawAsync_WithValidRequest_WithdrawsAndCreatesTransaction()
    {
        // Arrange
        var accountBefore = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        var balanceBefore = accountBefore.Balance;
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

        var accountAfter = _dataStore.Accounts.First(a => a.Id == _sarahCheckingId);
        accountAfter.Balance.Should().Be(balanceBefore - withdrawAmount);
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

    [Fact]
    public async Task WithdrawAsync_WithDefaultDescription_UsesCashWithdrawal()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 50.00m
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("Cash Withdrawal");
    }

    [Fact]
    public async Task WithdrawAsync_GeneratesReferenceNumber()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            AccountId = _sarahCheckingId,
            Amount = 50.00m
        };

        // Act
        var result = await _sut.WithdrawAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.ReferenceNumber.Should().StartWith("TRB");
    }

    #endregion
}
