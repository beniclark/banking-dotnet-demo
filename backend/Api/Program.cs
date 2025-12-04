using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using ThreeRiversBank.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Create a single SQLite in-memory connection that stays open for the lifetime of the app
var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

// Add DbContext with SQLite in-memory database
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlite(connection));

// Add services to the container
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Three Rivers Bank API",
        Version = "v1",
        Description = "A retail banking API for Three Rivers Bank - Demo Application",
        Contact = new OpenApiContact
        {
            Name = "Three Rivers Bank",
            Email = "support@threeriversbank.com"
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
    DatabaseSeeder.SeedDatabase(dbContext);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Three Rivers Bank API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Three Rivers Bank API";
    });
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

// API Routes
var api = app.MapGroup("/api");

// Customer endpoints
api.MapGet("/customers", async (ICustomerService customerService) =>
{
    var customers = await customerService.GetAllCustomersAsync();
    return Results.Ok(customers);
}).WithName("GetAllCustomers").WithTags("Customers");

api.MapGet("/customers/{customerId:guid}", async (Guid customerId, ICustomerService customerService) =>
{
    var customer = await customerService.GetCustomerByIdAsync(customerId);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
}).WithName("GetCustomerById").WithTags("Customers");

api.MapGet("/customers/{customerId:guid}/profile", async (Guid customerId, ICustomerService customerService) =>
{
    var profile = await customerService.GetCustomerProfileAsync(customerId);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
}).WithName("GetCustomerProfile").WithTags("Customers");

// Account endpoints
api.MapGet("/customers/{customerId:guid}/accounts", async (Guid customerId, IAccountService accountService) =>
{
    var accounts = await accountService.GetAccountsByCustomerIdAsync(customerId);
    return Results.Ok(accounts);
}).WithName("GetCustomerAccounts").WithTags("Accounts");

api.MapGet("/accounts/{accountId:guid}", async (Guid accountId, IAccountService accountService) =>
{
    var account = await accountService.GetAccountByIdAsync(accountId);
    return account is not null ? Results.Ok(account) : Results.NotFound();
}).WithName("GetAccountById").WithTags("Accounts");

// Transaction endpoints
api.MapGet("/accounts/{accountId:guid}/transactions", async (Guid accountId, int? count, ITransactionService transactionService) =>
{
    var transactions = await transactionService.GetTransactionsByAccountIdAsync(accountId, count ?? 50);
    return Results.Ok(transactions);
}).WithName("GetAccountTransactions").WithTags("Transactions");

api.MapGet("/transactions/{transactionId:guid}", async (Guid transactionId, ITransactionService transactionService) =>
{
    var transaction = await transactionService.GetTransactionByIdAsync(transactionId);
    return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
}).WithName("GetTransactionById").WithTags("Transactions");

// Banking operations
api.MapPost("/transfer", async (TransferRequest request, ITransactionService transactionService) =>
{
    var result = await transactionService.TransferFundsAsync(request);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
}).WithName("TransferFunds").WithTags("Banking Operations");

api.MapPost("/deposit", async (DepositRequest request, ITransactionService transactionService) =>
{
    var transaction = await transactionService.DepositAsync(request);
    return transaction is not null ? Results.Ok(transaction) : Results.BadRequest("Deposit failed");
}).WithName("Deposit").WithTags("Banking Operations");

api.MapPost("/withdraw", async (WithdrawalRequest request, ITransactionService transactionService) =>
{
    var transaction = await transactionService.WithdrawAsync(request);
    return transaction is not null ? Results.Ok(transaction) : Results.BadRequest("Withdrawal failed");
}).WithName("Withdraw").WithTags("Banking Operations");

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Bank = "Three Rivers Bank", Version = "1.0.0" }))
   .WithName("HealthCheck");

app.Run();
