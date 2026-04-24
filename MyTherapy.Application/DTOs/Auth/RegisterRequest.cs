using MyTherapy.Domain.Enums;

namespace MyTherapy.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsAnonymous { get; set; } = false;
}
