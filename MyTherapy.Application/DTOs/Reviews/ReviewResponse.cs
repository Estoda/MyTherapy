using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Reviews;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
