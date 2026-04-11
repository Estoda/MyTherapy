using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Phone {get; set;}
    public Gender? Gender {get; set;}
    public DateTime? DateOfBirth {get; set;}
    public string? ProfilePictureUrl {get; set;}
    public bool IsAnonymous {get; set;} = false; // default to false
    public Role Role { get; set; }
    public UserStatus Status {get; set;} = UserStatus.Active; // default to Active
    public DateTime? LastLogin {get; set;}
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
    
    // Navigation properties
    public PatientProfile? PatientProfile {get; set;}
    public TherapistProfile? TherapistProfile {get; set;}
    public AdminProfile? AdminProfile {get; set;}
}
