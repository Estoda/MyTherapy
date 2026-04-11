namespace MyTherapy.Domain.Enums;

public enum AppointmentStatus
{
    Cancelled = 1,
    Completed = 2,
    Scheduled = 3,
    NoShow = 4 // patients who do not show up for their scheduled appointments without prior notice
}
