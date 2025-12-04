using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

namespace ThreeRiversBank.Api.Services;

public class TransactionService : ITransactionService
{
    private readonly BankingDbContext _dbContext;

    public TransactionService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, int count = 50)
    {
        return await _dbContext.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        return await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<TransferResult> TransferFundsAsync(TransferRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId);
            var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId);

            if (fromAccount == null || toAccount == null)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = "One or both accounts not found"
                };
            }

            if (fromAccount.Balance < request.Amount)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = "Insufficient funds"
                };
            }

            if (request.Amount <= 0)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = "Transfer amount must be greater than zero"
                };
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
                    ? $"Transfer to account ending in {toAccount.AccountNumber.Substring(toAccount.AccountNumber.Length - 4)}"
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
                    ? $"Transfer from account ending in {fromAccount.AccountNumber.Substring(fromAccount.AccountNumber.Length - 4)}"
                    : request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Completed",
                ReferenceNumber = referenceNumber
            };

            _dbContext.Transactions.Add(fromTransaction);
            _dbContext.Transactions.Add(toTransaction);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new TransferResult
            {
                Success = true,
                Message = "Transfer completed successfully",
                ReferenceNumber = referenceNumber,
                FromTransaction = fromTransaction,
                ToTransaction = toTransaction
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Transaction?> DepositAsync(DepositRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId);
            if (account == null) return null;

            if (request.Amount <= 0) return null;

            account.Balance += request.Amount;
            account.AvailableBalance += request.Amount;

            var depositTransaction = new Transaction
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

            _dbContext.Transactions.Add(depositTransaction);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return depositTransaction;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Transaction?> WithdrawAsync(WithdrawalRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId);
            if (account == null) return null;

            if (request.Amount <= 0 || account.Balance < request.Amount)
                return null;

            account.Balance -= request.Amount;
            account.AvailableBalance -= request.Amount;

            var withdrawalTransaction = new Transaction
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

            _dbContext.Transactions.Add(withdrawalTransaction);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return withdrawalTransaction;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static string GenerateReferenceNumber()
    {
        return $"TRB{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(100000, 999999)}";
    }
}
