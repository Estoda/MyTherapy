using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Users;

public class UserSummaryResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}
