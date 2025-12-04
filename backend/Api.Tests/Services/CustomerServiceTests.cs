using FluentAssertions;
using ThreeRiversBank.Api.Services;
using ThreeRiversBank.Api.Services.Data;
using Xunit;

namespace ThreeRiversBank.Api.Tests.Services;

public class CustomerServiceTests : IDisposable
{
    private readonly CustomerService _sut;
    private readonly TestDatabaseFactory _dbFactory;

    // Known demo customer IDs
    private readonly Guid _sarahId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _michaelId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _emilyId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public CustomerServiceTests()
    {
        _dbFactory = new TestDatabaseFactory();
        _sut = new CustomerService(_dbFactory.DbContext);
    }

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    #region GetCustomerByIdAsync Tests

    [Fact]
    public async Task GetCustomerByIdAsync_WithValidId_ReturnsCustomer()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Sarah");
        result.LastName.Should().Be("Johnson");
        result.Email.Should().Be("sarah.johnson@email.com");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithMichaelId_ReturnsMichael()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(_michaelId);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Michael");
        result.LastName.Should().Be("Chen");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WithEmilyId_ReturnsEmily()
    {
        // Act
        var result = await _sut.GetCustomerByIdAsync(_emilyId);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Emily");
        result.LastName.Should().Be("Rodriguez");
    }

    #endregion

    #region GetAllCustomersAsync Tests

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllDemoCustomers()
    {
        // Act
        var result = await _sut.GetAllCustomersAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.FirstName == "Sarah");
        result.Should().Contain(c => c.FirstName == "Michael");
        result.Should().Contain(c => c.FirstName == "Emily");
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsCustomersWithCorrectProperties()
    {
        // Act
        var result = await _sut.GetAllCustomersAsync();

        // Assert
        result.Should().OnlyContain(c => !string.IsNullOrEmpty(c.Email));
        result.Should().OnlyContain(c => !string.IsNullOrEmpty(c.CustomerNumber));
        result.Should().OnlyContain(c => c.Id != Guid.Empty);
    }

    #endregion

    #region GetCustomerProfileAsync Tests

    [Fact]
    public async Task GetCustomerProfileAsync_WithValidId_ReturnsProfileWithAccounts()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Sarah Johnson");
        result.Accounts.Should().HaveCount(2);
        result.Accounts.Should().Contain(a => a.AccountType == "Checking");
        result.Accounts.Should().Contain(a => a.AccountType == "Savings");
    }

    [Fact]
    public async Task GetCustomerProfileAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomerProfileAsync_ReturnsMaskedAccountNumbers()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.Accounts.Should().OnlyContain(a => a.AccountNumber.StartsWith("****"));
    }

    [Fact]
    public async Task GetCustomerProfileAsync_ReturnsCorrectCustomerNumber()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerNumber.Should().Be("TRB-100001");
    }

    [Fact]
    public async Task GetCustomerProfileAsync_ReturnsCorrectMemberSinceDate()
    {
        // Act
        var result = await _sut.GetCustomerProfileAsync(_sarahId);

        // Assert
        result.Should().NotBeNull();
        result!.MemberSince.Should().Be(new DateTime(2019, 6, 10));
    }

    #endregion
}
