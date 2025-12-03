namespace ThreeRiversBank.Api.Models;

public class Account
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty; // Checking, Savings, etc.
    public string AccountName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OpenedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Currency { get; set; } = "USD";
}

public class AccountSummary
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
