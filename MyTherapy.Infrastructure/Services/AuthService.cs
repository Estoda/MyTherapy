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
    private readonly IEmailService _emailService;

    public AuthService(AppDbContext context, IConfiguration config, IEmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    public async Task<AuthResponse> RegisterPatientAsync(RegisterRequest request)
    {
        var verified = await _context.EmailVerifications
            .FirstOrDefaultAsync(e => e.Email == request.Email && e.IsVerified) ?? throw new ArgumentException("Email has not been verified. Please verify your email first.");
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Patient,
            Gender = request.Gender,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            IsAnonymous = request.IsAnonymous
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var patientProfile = new PatientProfile
        {
            UserId = user.Id
        };

        _context.Patients.Add(patientProfile);

        _context.EmailVerifications.Remove(verified);

        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<AuthResponse> RegisterTherapistAsync(RegisterTherapistRequest request)
    {
        var verified = await _context.EmailVerifications
            .FirstOrDefaultAsync(e => e.Email == request.Email && e.IsVerified) ?? throw new ArgumentException("Email has not been verified. Please verify your email first.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.Therapist,
            Gender = request.Gender,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            IsAnonymous = request.IsAnonymous
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var therapistProfile = new TherapistProfile
        {
            UserId = user.Id,
            Specialization = request.Specialization,
            LicenseNumber = request.LicenseNumber,
            LicenseDocumentPath = string.Empty,
            ExperienceYears = 0,
            Available = true,
            PricePerSession = 500
        };

        _context.Therapists.Add(therapistProfile);

        _context.EmailVerifications.Remove(verified);

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

    public async Task SendVerificationCodeAsync(string email)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        if (exists)
            throw new ArgumentException("Email is already registered!");

        var code = Random.Shared.Next(100000, 999999).ToString();

        var existing = await _context.EmailVerifications
            .Where(e => e.Email == email)
            .ToListAsync();

        _context.EmailVerifications.RemoveRange(existing);

        var verfication = new EmailVerification
        {
            Email = email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsVerified = false
        };
        _context.EmailVerifications.Add(verfication);

        await _context.SaveChangesAsync();
        await _emailService.SendVerificationCodeAsync(email, code);

    }

    public async Task<bool> VerifyEmailCodeAsync(string email, string code)
    {
        var record = await _context.EmailVerifications
            .FirstOrDefaultAsync(e => e.Email == email);

        if (record == null)
            throw new KeyNotFoundException("No verification request found for this email.");

        if (record.ExpiresAt < DateTime.UtcNow)
            throw new ArgumentException("Verification code has expired.");

        if (record.Code != code)
            throw new ArgumentException("Invalid verification code.");

        record.IsVerified = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
