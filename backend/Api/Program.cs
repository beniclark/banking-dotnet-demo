using Microsoft.OpenApi.Models;
using ThreeRiversBank.Api.Models;
using ThreeRiversBank.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IBankingService, BankingService>();
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
api.MapGet("/customers", async (IBankingService bankingService) =>
{
    var customers = await bankingService.GetAllCustomersAsync();
    return Results.Ok(customers);
}).WithName("GetAllCustomers").WithTags("Customers");

api.MapGet("/customers/{customerId:guid}", async (Guid customerId, IBankingService bankingService) =>
{
    var customer = await bankingService.GetCustomerByIdAsync(customerId);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
}).WithName("GetCustomerById").WithTags("Customers");

api.MapGet("/customers/{customerId:guid}/profile", async (Guid customerId, IBankingService bankingService) =>
{
    var profile = await bankingService.GetCustomerProfileAsync(customerId);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
}).WithName("GetCustomerProfile").WithTags("Customers");

// Account endpoints
api.MapGet("/customers/{customerId:guid}/accounts", async (Guid customerId, IBankingService bankingService) =>
{
    var accounts = await bankingService.GetAccountsByCustomerIdAsync(customerId);
    return Results.Ok(accounts);
}).WithName("GetCustomerAccounts").WithTags("Accounts");

api.MapGet("/accounts/{accountId:guid}", async (Guid accountId, IBankingService bankingService) =>
{
    var account = await bankingService.GetAccountByIdAsync(accountId);
    return account is not null ? Results.Ok(account) : Results.NotFound();
}).WithName("GetAccountById").WithTags("Accounts");

// Transaction endpoints
api.MapGet("/accounts/{accountId:guid}/transactions", async (Guid accountId, int? count, IBankingService bankingService) =>
{
    var transactions = await bankingService.GetTransactionsByAccountIdAsync(accountId, count ?? 50);
    return Results.Ok(transactions);
}).WithName("GetAccountTransactions").WithTags("Transactions");

api.MapGet("/transactions/{transactionId:guid}", async (Guid transactionId, IBankingService bankingService) =>
{
    var transaction = await bankingService.GetTransactionByIdAsync(transactionId);
    return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
}).WithName("GetTransactionById").WithTags("Transactions");

// Banking operations
api.MapPost("/transfer", async (TransferRequest request, IBankingService bankingService) =>
{
    var result = await bankingService.TransferFundsAsync(request);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
}).WithName("TransferFunds").WithTags("Banking Operations");

api.MapPost("/deposit", async (DepositRequest request, IBankingService bankingService) =>
{
    var transaction = await bankingService.DepositAsync(request);
    return transaction is not null ? Results.Ok(transaction) : Results.BadRequest("Deposit failed");
}).WithName("Deposit").WithTags("Banking Operations");

api.MapPost("/withdraw", async (WithdrawalRequest request, IBankingService bankingService) =>
{
    var transaction = await bankingService.WithdrawAsync(request);
    return transaction is not null ? Results.Ok(transaction) : Results.BadRequest("Withdrawal failed");
}).WithName("Withdraw").WithTags("Banking Operations");

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Bank = "Three Rivers Bank", Version = "1.0.0" }))
   .WithName("HealthCheck");

app.Run();
