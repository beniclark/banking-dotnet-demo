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
- **In-memory data store** - For demo purposes

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
│       │   │   └── BankingDataStore.cs    # Shared in-memory data store
│       │   ├── Interfaces/
│       │   │   ├── IAccountService.cs
│       │   │   ├── ICustomerService.cs
│       │   │   └── ITransactionService.cs
│       │   ├── AccountService.cs          # Account domain operations
│       │   ├── CustomerService.cs         # Customer domain operations
│       │   └── TransactionService.cs      # Transaction domain operations
│       └── Program.cs
│   └── Api.Tests/
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

All services share a common `BankingDataStore` for in-memory data persistence.

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

The application comes with three pre-configured demo customers:

| Customer | Email | Customer Number |
|----------|-------|-----------------|
| Sarah Johnson | sarah.johnson@email.com | TRB-100001 |
| Michael Chen | michael.chen@email.com | TRB-100002 |
| Emily Rodriguez | emily.rodriguez@email.com | TRB-100003 |

**Default demo user:** Sarah Johnson (ID: `11111111-1111-1111-1111-111111111111`)

## 🎯 Features

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
3. **Add database**: "Connect to SQL Server database"
4. **Add tests**: "Write unit tests for the banking service"
5. **Add validation**: "Add input validation to the transfer endpoint"
6. **Refactor code**: "Extract account operations to a separate service"

## 📝 License

This project is for demonstration purposes only.

---

Made with ❤️ for GitHub Copilot demos
