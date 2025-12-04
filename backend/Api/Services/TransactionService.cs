using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class TransactionService : ITransactionService
{
    private readonly IBankingDataStore _dataStore;

    public TransactionService(IBankingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, int count = 50)
    {
        var transactions = _dataStore.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .ToList();

        return Task.FromResult(transactions);
    }

    public Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = _dataStore.Transactions.FirstOrDefault(t => t.Id == transactionId);
        return Task.FromResult(transaction);
    }

    public Task<TransferResult> TransferFundsAsync(TransferRequest request)
    {
        lock (_dataStore.Lock)
        {
            var fromAccount = _dataStore.Accounts.FirstOrDefault(a => a.Id == request.FromAccountId);
            var toAccount = _dataStore.Accounts.FirstOrDefault(a => a.Id == request.ToAccountId);

            if (fromAccount == null || toAccount == null)
            {
                return Task.FromResult(new TransferResult
                {
                    Success = false,
                    Message = "One or both accounts not found"
                });
            }

            if (fromAccount.Balance < request.Amount)
            {
                return Task.FromResult(new TransferResult
                {
                    Success = false,
                    Message = "Insufficient funds"
                });
            }

            if (request.Amount <= 0)
            {
                return Task.FromResult(new TransferResult
                {
                    Success = false,
                    Message = "Transfer amount must be greater than zero"
                });
            }

            var referenceNumber = GenerateReferenceNumber();

            // Debit from source account
            fromAccount.Balance -= request.Amount;
            fromAccount.AvailableBalance -= request.Amount;

            var fromTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = fromAccount.Id,
                TransactionType = "Debit",
                Category = "Transfer",
                Amount = request.Amount,
                BalanceAfter = fromAccount.Balance,
                Description = string.IsNullOrEmpty(request.Description)
                    ? $"Transfer to account ending in {toAccount.AccountNumber[^4..]}"
                    : request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                ReferenceNumber = referenceNumber
            };

            // Credit to destination account
            toAccount.Balance += request.Amount;
            toAccount.AvailableBalance += request.Amount;

            var toTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = toAccount.Id,
                TransactionType = "Credit",
                Category = "Transfer",
                Amount = request.Amount,
                BalanceAfter = toAccount.Balance,
                Description = string.IsNullOrEmpty(request.Description)
                    ? $"Transfer from account ending in {fromAccount.AccountNumber[^4..]}"
                    : request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                ReferenceNumber = referenceNumber
            };

            _dataStore.Transactions.Add(fromTransaction);
            _dataStore.Transactions.Add(toTransaction);

            return Task.FromResult(new TransferResult
            {
                Success = true,
                Message = "Transfer completed successfully",
                ReferenceNumber = referenceNumber,
                FromTransaction = fromTransaction,
                ToTransaction = toTransaction
            });
        }
    }

    public Task<Transaction?> DepositAsync(DepositRequest request)
    {
        lock (_dataStore.Lock)
        {
            var account = _dataStore.Accounts.FirstOrDefault(a => a.Id == request.AccountId);
            if (account == null) return Task.FromResult<Transaction?>(null);

            if (request.Amount <= 0) return Task.FromResult<Transaction?>(null);

            account.Balance += request.Amount;
            account.AvailableBalance += request.Amount;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                TransactionType = "Credit",
                Category = "Deposit",
                Amount = request.Amount,
                BalanceAfter = account.Balance,
                Description = string.IsNullOrEmpty(request.Description) ? "Cash Deposit" : request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                ReferenceNumber = GenerateReferenceNumber()
            };

            _dataStore.Transactions.Add(transaction);
            return Task.FromResult<Transaction?>(transaction);
        }
    }

    public Task<Transaction?> WithdrawAsync(WithdrawalRequest request)
    {
        lock (_dataStore.Lock)
        {
            var account = _dataStore.Accounts.FirstOrDefault(a => a.Id == request.AccountId);
            if (account == null) return Task.FromResult<Transaction?>(null);

            if (request.Amount <= 0 || account.Balance < request.Amount)
                return Task.FromResult<Transaction?>(null);

            account.Balance -= request.Amount;
            account.AvailableBalance -= request.Amount;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                TransactionType = "Debit",
                Category = "Withdrawal",
                Amount = request.Amount,
                BalanceAfter = account.Balance,
                Description = string.IsNullOrEmpty(request.Description) ? "Cash Withdrawal" : request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                ReferenceNumber = GenerateReferenceNumber()
            };

            _dataStore.Transactions.Add(transaction);
            return Task.FromResult<Transaction?>(transaction);
        }
    }

    private static string GenerateReferenceNumber()
    {
        return $"TRB{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(100000, 999999)}";
    }
}
