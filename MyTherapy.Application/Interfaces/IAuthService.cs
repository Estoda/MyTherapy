using MyTherapy.Application.DTOs.Auth;

namespace MyTherapy.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterPatientAsync(RegisterRequest request);
    Task<AuthResponse> RegisterTherapistAsync(RegisterTherapistRequest request);
    Task SendVerificationCodeAsync(string email);
    Task<bool> VerifyEmailCodeAsync(string email, string code);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
