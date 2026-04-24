namespace MyTherapy.Application.DTOs.Auth;

public class RegisterTherapistRequest : RegisterRequest
{
    public string Specialization { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public string LicenseDocumentPath { get; set; } = null!;
}
