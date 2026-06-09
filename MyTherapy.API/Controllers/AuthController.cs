using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTherapy.Application.DTOs.Auth;
using MyTherapy.Application.Interfaces;

namespace MyTherapy.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register/patient")]
    public async Task<IActionResult> RegisterPatient(RegisterRequest request)
        => Ok(await _authService.RegisterPatientAsync(request));

    [HttpPost("register/therapist")]
    public async Task<IActionResult> RegisterTherapist(RegisterTherapistRequest request)
        => Ok(await _authService.RegisterTherapistAsync(request));

    [HttpPost("login")]
    public async Task<IActionResult> login(LoginRequest request)
        => Ok(await _authService.LoginAsync(request));

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationcode([FromBody] SendVerificationCodeRequest request)
    {
        await _authService.SendVerificationCodeAsync(request.Email);
        return Ok(new { message = "Verification code sent to your email." });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _authService.VerifyEmailCodeAsync(request.Email, request.Code);
        return Ok(new { message = "Email verified successfully. You may now register." });

    }
}
