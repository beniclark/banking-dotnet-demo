# Three Rivers Bank - Demo Application

A retail banking demo application built with .NET 10 and React, designed for GitHub Copilot demos.

![Three Rivers Bank](https://img.shields.io/badge/Three%20Rivers%20Bank-Demo-blue)

## 🏦 About

Three Rivers Bank is a fictional retail banking application that demonstrates modern web development practices. It features a .NET 10 backend API and a React frontend with TypeScript.

## 🛠️ Tech Stack

### Backend
- **.NET 10** - Latest .NET framework
- **C#** - Primary programming language
- **Minimal APIs** - Modern API design pattern
- **Entity Framework Core** - ORM for data access
- **SQLite In-Memory** - Lightweight database for demo purposes

### Frontend
- **React 18** - UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool
- **React Router** - Client-side routing
- **Axios** - HTTP client

## 📁 Project Structure

```
banking-dotnet-demo/
├── backend/
│   └── Api/
│       ├── Models/
│       │   ├── Account.cs
│       │   ├── Customer.cs
│       │   └── Transaction.cs
│       ├── Services/
│       │   ├── Data/
│       │   │   ├── BankingDbContext.cs    # EF Core DbContext
│       │   │   └── DatabaseSeeder.cs      # Seed data initialization
│       │   ├── Interfaces/
│       │   │   ├── IAccountService.cs
│       │   │   ├── ICustomerService.cs
│       │   │   └── ITransactionService.cs
│       │   ├── AccountService.cs          # Account domain operations
│       │   ├── CustomerService.cs         # Customer domain operations
│       │   └── TransactionService.cs      # Transaction domain operations
│       └── Program.cs
│   └── Api.Tests/
│       ├── TestDatabaseFactory.cs         # Test database setup
│       └── Services/
│           ├── AccountServiceTests.cs
│           ├── CustomerServiceTests.cs
│           └── TransactionServiceTests.cs
├── frontend/
│   └── ui/
│       ├── src/
│       │   ├── components/
│       │   ├── pages/
│       │   ├── services/
│       │   └── types/
│       └── package.json
└── README.md
```

## 🏛️ Service Architecture

The backend follows a **domain-driven modular architecture** with three main services:

| Service | Responsibility |
|---------|----------------|
| `CustomerService` | Customer lookup, profile management |
| `AccountService` | Account queries, balance summaries |
| `TransactionService` | Transactions, transfers, deposits, withdrawals |

### Data Layer

The application uses **Entity Framework Core** with **SQLite in-memory database**:

| Component | Description |
|-----------|-------------|
| `BankingDbContext` | EF Core DbContext with entity configurations |
| `DatabaseSeeder` | Initializes the database with demo data on startup |

The SQLite in-memory database is created fresh on each application start, seeded with demo customers, accounts, and transactions. This approach provides:
- Real database operations with SQL queries
- Transaction support for data integrity
- Easy testing with isolated database instances
- No external database dependencies

### Database Schema

```mermaid
erDiagram
    Customer {
        guid Id PK
        string FirstName
        string LastName
        string Email UK
        string Phone
        string Address
        string City
        string State
        string ZipCode
        datetime DateOfBirth
        datetime CreatedDate
        bool IsActive
        string CustomerNumber UK
    }
    
    Account {
        guid Id PK
        string AccountNumber UK
        string AccountType
        string AccountName
        decimal Balance
        decimal AvailableBalance
        guid CustomerId FK
        datetime OpenedDate
        bool IsActive
        string Currency
    }
    
    Transaction {
        guid Id PK
        guid AccountId FK
        string TransactionType
        string Category
        decimal Amount
        decimal BalanceAfter
        string Description
        string Merchant
        datetime TransactionDate
        string Status
        string ReferenceNumber
    }
    
    Customer ||--o{ Account : "has"
    Account ||--o{ Transaction : "has"
```

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- npm or yarn

### Running the Backend

```bash
cd backend/Api
dotnet run
```

The API will start at `http://localhost:5000`

### Running the Frontend

```bash
cd frontend/ui
npm install
npm run dev
```

The frontend will start at `http://localhost:5173`

## 📡 API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate user with email

### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `GET /api/customers/{id}/profile` - Get customer profile with accounts

### Accounts
- `GET /api/customers/{id}/accounts` - Get customer accounts
- `GET /api/accounts/{id}` - Get account by ID

### Transactions
- `GET /api/accounts/{id}/transactions` - Get account transactions
- `GET /api/transactions/{id}` - Get transaction by ID

### Banking Operations
- `POST /api/transfer` - Transfer funds between accounts
- `POST /api/deposit` - Deposit funds
- `POST /api/withdraw` - Withdraw funds

### Health Check
- `GET /health` - API health status

## 👥 Demo Customers

The application comes with three pre-configured demo customers. **You must log in to access the application.**

| Customer | Email | Customer Number |
|----------|-------|-----------------|
| Sarah Johnson | sarah.johnson@email.com | TRB-100001 |
| Michael Chen | michael.chen@email.com | TRB-100002 |
| Emily Rodriguez | emily.rodriguez@email.com | TRB-100003 |

### Logging In

1. Navigate to `http://localhost:5173` (the app will redirect to the login page)
2. Enter one of the demo email addresses above, or use the quick login buttons
3. Click "Log In" to access your account
4. Use the "Logout" button in the header to log out

**Note:** This is a demo application with simplified authentication - only email addresses are required (no passwords).

## 🎯 Features

- **User Authentication** - Secure login with email-based authentication
- **Dashboard** - Overview of accounts and recent transactions
- **Account Management** - View account details and balances
- **Fund Transfers** - Transfer money between accounts
- **Transaction History** - View all transactions with filtering

## 🎨 UI Components

- **Header** - Navigation and branding
- **AccountCard** - Display account information
- **TransactionList** - List of transactions with icons

## 💡 Copilot Demo Ideas

1. **Add new feature**: "Add a bill payment feature"
2. **Add authentication**: "Implement JWT authentication"
3. **Migrate database**: "Connect to Azure SQL database"
4. **Add tests**: "Write unit tests for the banking service"
5. **Add validation**: "Add input validation to the transfer endpoint"
6. **Refactor code**: "Extract account operations to a separate service"

## 📝 License

This project is for demonstration purposes only.

---

Made with ❤️ for GitHub Copilot demos
