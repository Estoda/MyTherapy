using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Reviews;

public class CreateReviewRequest
{
    public Guid AppointmentId { get; set; }
    public int Rating { get; set; } 
    public string? Comment { get; set; }
}
