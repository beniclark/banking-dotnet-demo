using ThreeRiversBank.Api.Models;

namespace ThreeRiversBank.Api.Services.Data;

public static class DatabaseSeeder
{
    public static void SeedDatabase(BankingDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Only seed if the database is empty
        if (context.Customers.Any())
        {
            return;
        }

        var customers = GetSeedCustomers();
        var accounts = GetSeedAccounts();
        var transactions = GetSeedTransactions();

        context.Customers.AddRange(customers);
        context.Accounts.AddRange(accounts);
        context.Transactions.AddRange(transactions);
        context.SaveChanges();
    }

    private static List<Customer> GetSeedCustomers()
    {
        return new List<Customer>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@email.com",
                Phone = "412-555-0101",
                Address = "123 Liberty Ave",
                City = "Pittsburgh",
                State = "PA",
                ZipCode = "15222",
                DateOfBirth = new DateTime(1985, 3, 15),
                CreatedDate = new DateTime(2019, 6, 10),
                CustomerNumber = "TRB-100001"
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                FirstName = "Michael",
                LastName = "Chen",
                Email = "michael.chen@email.com",
                Phone = "412-555-0102",
                Address = "456 Forbes Ave",
                City = "Pittsburgh",
                State = "PA",
                ZipCode = "15213",
                DateOfBirth = new DateTime(1990, 7, 22),
                CreatedDate = new DateTime(2020, 2, 15),
                CustomerNumber = "TRB-100002"
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                FirstName = "Emily",
                LastName = "Rodriguez",
                Email = "emily.rodriguez@email.com",
                Phone = "412-555-0103",
                Address = "789 Fifth Ave",
                City = "Pittsburgh",
                State = "PA",
                ZipCode = "15232",
                DateOfBirth = new DateTime(1988, 11, 8),
                CreatedDate = new DateTime(2021, 9, 1),
                CustomerNumber = "TRB-100003"
            }
        };
    }

    private static List<Account> GetSeedAccounts()
    {
        return new List<Account>
        {
            // Sarah's accounts
            new()
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                AccountNumber = "1001234567",
                AccountType = "Checking",
                AccountName = "Primary Checking",
                Balance = 5432.87m,
                AvailableBalance = 5432.87m,
                CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                OpenedDate = new DateTime(2019, 6, 10),
                Currency = "USD"
            },
            new()
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                AccountNumber = "1001234568",
                AccountType = "Savings",
                AccountName = "Emergency Fund",
                Balance = 15750.00m,
                AvailableBalance = 15750.00m,
                CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                OpenedDate = new DateTime(2019, 8, 15),
                Currency = "USD"
            },
            // Michael's accounts
            new()
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                AccountNumber = "1002345678",
                AccountType = "Checking",
                AccountName = "Main Checking",
                Balance = 3218.45m,
                AvailableBalance = 3218.45m,
                CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                OpenedDate = new DateTime(2020, 2, 15),
                Currency = "USD"
            },
            new()
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                AccountNumber = "1002345679",
                AccountType = "Savings",
                AccountName = "Vacation Savings",
                Balance = 8500.00m,
                AvailableBalance = 8500.00m,
                CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                OpenedDate = new DateTime(2020, 5, 20),
                Currency = "USD"
            },
            // Emily's accounts
            new()
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                AccountNumber = "1003456789",
                AccountType = "Checking",
                AccountName = "Everyday Checking",
                Balance = 2876.32m,
                AvailableBalance = 2876.32m,
                CustomerId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                OpenedDate = new DateTime(2021, 9, 1),
                Currency = "USD"
            },
            new()
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                AccountNumber = "1003456790",
                AccountType = "Savings",
                AccountName = "Home Down Payment",
                Balance = 45000.00m,
                AvailableBalance = 45000.00m,
                CustomerId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                OpenedDate = new DateTime(2021, 10, 15),
                Currency = "USD"
            }
        };
    }

    private static List<Transaction> GetSeedTransactions()
    {
        var transactions = new List<Transaction>();
        var baseDate = DateTime.UtcNow;

        // Sarah's checking transactions
        var sarahCheckingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        transactions.AddRange(new[]
        {
            new Transaction
            {
                Id = Guid.Parse("10000001-0001-0001-0001-000000000001"),
                AccountId = sarahCheckingId,
                TransactionType = "Credit",
                Category = "Deposit",
                Amount = 3500.00m,
                BalanceAfter = 5432.87m,
                Description = "Direct Deposit - Employer",
                Merchant = "ACME Corp",
                TransactionDate = baseDate.AddDays(-2),
                Status = "Completed",
                ReferenceNumber = "TRB202312010001"
            },
            new Transaction
            {
                Id = Guid.Parse("10000001-0001-0001-0001-000000000002"),
                AccountId = sarahCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 45.67m,
                BalanceAfter = 1932.87m,
                Description = "Grocery Store",
                Merchant = "Giant Eagle",
                TransactionDate = baseDate.AddDays(-3),
                Status = "Completed",
                ReferenceNumber = "TRB202312010002"
            },
            new Transaction
            {
                Id = Guid.Parse("10000001-0001-0001-0001-000000000003"),
                AccountId = sarahCheckingId,
                TransactionType = "Debit",
                Category = "Payment",
                Amount = 150.00m,
                BalanceAfter = 1978.54m,
                Description = "Electric Bill Payment",
                Merchant = "Duquesne Light",
                TransactionDate = baseDate.AddDays(-5),
                Status = "Completed",
                ReferenceNumber = "TRB202312010003"
            },
            new Transaction
            {
                Id = Guid.Parse("10000001-0001-0001-0001-000000000004"),
                AccountId = sarahCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 89.99m,
                BalanceAfter = 2128.54m,
                Description = "Online Shopping",
                Merchant = "Amazon",
                TransactionDate = baseDate.AddDays(-7),
                Status = "Completed",
                ReferenceNumber = "TRB202312010004"
            },
            new Transaction
            {
                Id = Guid.Parse("10000001-0001-0001-0001-000000000005"),
                AccountId = sarahCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 35.50m,
                BalanceAfter = 2218.53m,
                Description = "Restaurant",
                Merchant = "Primanti Bros",
                TransactionDate = baseDate.AddDays(-8),
                Status = "Completed",
                ReferenceNumber = "TRB202312010005"
            }
        });

        // Michael's checking transactions
        var michaelCheckingId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        transactions.AddRange(new[]
        {
            new Transaction
            {
                Id = Guid.Parse("10000002-0002-0002-0002-000000000001"),
                AccountId = michaelCheckingId,
                TransactionType = "Credit",
                Category = "Deposit",
                Amount = 2800.00m,
                BalanceAfter = 3218.45m,
                Description = "Direct Deposit - Salary",
                Merchant = "Tech Solutions Inc",
                TransactionDate = baseDate.AddDays(-1),
                Status = "Completed",
                ReferenceNumber = "TRB202312020001"
            },
            new Transaction
            {
                Id = Guid.Parse("10000002-0002-0002-0002-000000000002"),
                AccountId = michaelCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 125.00m,
                BalanceAfter = 418.45m,
                Description = "Gas Station",
                Merchant = "GetGo",
                TransactionDate = baseDate.AddDays(-4),
                Status = "Completed",
                ReferenceNumber = "TRB202312020002"
            },
            new Transaction
            {
                Id = Guid.Parse("10000002-0002-0002-0002-000000000003"),
                AccountId = michaelCheckingId,
                TransactionType = "Debit",
                Category = "Transfer",
                Amount = 500.00m,
                BalanceAfter = 543.45m,
                Description = "Transfer to Savings",
                Merchant = "",
                TransactionDate = baseDate.AddDays(-6),
                Status = "Completed",
                ReferenceNumber = "TRB202312020003"
            }
        });

        // Emily's checking transactions
        var emilyCheckingId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        transactions.AddRange(new[]
        {
            new Transaction
            {
                Id = Guid.Parse("10000003-0003-0003-0003-000000000001"),
                AccountId = emilyCheckingId,
                TransactionType = "Credit",
                Category = "Deposit",
                Amount = 4200.00m,
                BalanceAfter = 2876.32m,
                Description = "Direct Deposit - Payroll",
                Merchant = "Healthcare Partners",
                TransactionDate = baseDate.AddDays(-1),
                Status = "Completed",
                ReferenceNumber = "TRB202312030001"
            },
            new Transaction
            {
                Id = Guid.Parse("10000003-0003-0003-0003-000000000002"),
                AccountId = emilyCheckingId,
                TransactionType = "Debit",
                Category = "Payment",
                Amount = 1500.00m,
                BalanceAfter = -1323.68m,
                Description = "Rent Payment",
                Merchant = "Property Management LLC",
                TransactionDate = baseDate.AddDays(-2),
                Status = "Completed",
                ReferenceNumber = "TRB202312030002"
            },
            new Transaction
            {
                Id = Guid.Parse("10000003-0003-0003-0003-000000000003"),
                AccountId = emilyCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 67.89m,
                BalanceAfter = 176.32m,
                Description = "Pharmacy",
                Merchant = "CVS Pharmacy",
                TransactionDate = baseDate.AddDays(-4),
                Status = "Completed",
                ReferenceNumber = "TRB202312030003"
            },
            new Transaction
            {
                Id = Guid.Parse("10000003-0003-0003-0003-000000000004"),
                AccountId = emilyCheckingId,
                TransactionType = "Debit",
                Category = "Purchase",
                Amount = 250.00m,
                BalanceAfter = 244.21m,
                Description = "Clothing Store",
                Merchant = "Nordstrom",
                TransactionDate = baseDate.AddDays(-9),
                Status = "Completed",
                ReferenceNumber = "TRB202312030004"
            }
        });

        return transactions;
    }
}
