using MyTherapy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Domain.Entities;

public class Review : BaseEntity
{
    public Guid PatientId { get; set; }
    public PatientProfile Patient { get; set; } = null!;

    public Guid TherapistId { get; set; }
    public TherapistProfile Therapist { get; set; } = null!;

    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public int Rating { get; set; } // 1 to 5
    public string? Comment { get; set; }
}
