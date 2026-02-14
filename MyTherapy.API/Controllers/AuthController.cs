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
}
