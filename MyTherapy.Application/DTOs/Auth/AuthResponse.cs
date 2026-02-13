namespace MyTherapy.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public DateTime Epiration { get; set; }
}
