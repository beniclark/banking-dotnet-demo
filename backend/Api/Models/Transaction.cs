namespace ThreeRiversBank.Api.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Debit, Credit
    public string Category { get; set; } = string.Empty; // Transfer, Payment, Deposit, Withdrawal, Purchase
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Merchant { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; } = "Completed"; // Pending, Completed, Failed
    public string ReferenceNumber { get; set; } = string.Empty;
}

public class TransferRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class TransferResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public Transaction? FromTransaction { get; set; }
    public Transaction? ToTransaction { get; set; }
}

public class DepositRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class WithdrawalRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
