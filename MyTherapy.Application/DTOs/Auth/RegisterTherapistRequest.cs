namespace MyTherapy.Application.DTOs.Auth;

public class RegisterTherapistRequest : RegisterRequest
{
    public string LicenseNumber { get; set; } = null!;
    public string LicenseDucumentPath { get; set; } = null!;
}
