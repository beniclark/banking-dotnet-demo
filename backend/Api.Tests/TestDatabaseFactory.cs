using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ThreeRiversBank.Api.Services.Data;

namespace ThreeRiversBank.Api.Tests;

public class TestDatabaseFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public BankingDbContext DbContext { get; }

    public TestDatabaseFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new BankingDbContext(options);
        DatabaseSeeder.SeedDatabase(DbContext);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
