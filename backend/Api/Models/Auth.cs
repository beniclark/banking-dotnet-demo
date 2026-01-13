namespace ThreeRiversBank.Api.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public CustomerProfile? Customer { get; set; }
}
