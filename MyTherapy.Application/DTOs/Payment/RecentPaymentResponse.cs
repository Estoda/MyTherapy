using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Payment;

public class RecentPaymentResponse
{
    public string PatientName { get; set; } = string.Empty;
    public string? PatientProfilePicture { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
