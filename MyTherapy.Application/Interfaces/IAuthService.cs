using MyTherapy.Application.DTOs.Auth;

namespace MyTherapy.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterPatientAsync(RegisterRequest request);
    Task<AuthResponse> RegisterTherapistAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
