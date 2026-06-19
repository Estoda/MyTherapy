using System;
using System.Collections.Generic;
using System.Text;

namespace MyTherapy.Application.DTOs.Payment;

public class EarningsResponse
{
    public decimal TodayEarnings { get; set; }
    public decimal ThisMonthEarnings { get; set; }
    public decimal TotalEarnings { get; set; }

    public List<RecentPaymentResponse> RecentPayments { get; set; } = new(); 
}
