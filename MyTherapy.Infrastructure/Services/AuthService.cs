using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyTherapy.Application.DTOs.Auth;
using MyTherapy.Application.Interfaces;
using MyTherapy.Domain.Entities;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyTherapy.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> RegisterPatientAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new ArgumentException("Email already exists!");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Patient
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var patientProfile = new PatientProfile
        {
            UserId = user.Id
        };

        _context.Patients.Add(patientProfile);
        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<AuthResponse> RegisterTherapistAsync(RegisterTherapistRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new ArgumentException("Email already exists!");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Therapist
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var therapistProfile = new TherapistProfile
        {
            UserId = user.Id,
            Specialization = request.Specialization,
            LicenseNumber = request.LicenseNumber,
            LicenseDocumentPath = request.LicenseDocumentPath
        };

        _context.Therapists.Add(therapistProfile);
        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials!");

        return GenerateToken(user);
    }

    private AuthResponse GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"]!)),
                signingCredentials: creds
            );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        };
    }

}
