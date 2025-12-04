using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, int count = 50);
    Task<Transaction?> GetTransactionByIdAsync(Guid transactionId);
    Task<TransferResult> TransferFundsAsync(TransferRequest request);
    Task<Transaction?> DepositAsync(DepositRequest request);
    Task<Transaction?> WithdrawAsync(WithdrawalRequest request);
}
