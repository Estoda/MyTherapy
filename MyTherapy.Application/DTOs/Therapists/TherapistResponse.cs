using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Therapists
{
    public class TherapistResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? ProfilePicture { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string VerificationStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
