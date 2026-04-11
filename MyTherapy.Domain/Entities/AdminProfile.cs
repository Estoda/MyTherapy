using MyTherapy.Domain.Common;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Domain.Entities;

public class AdminProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Permissions { get; set; } = null!; // json string representing admin permissions, e.g. {"ManageUsers": true, "ManageTherapists": true, "ViewReports": true}
}
