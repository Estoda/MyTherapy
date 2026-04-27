
namespace MyTherapy.Application.DTOs.Payment;

public class PaymentInitiateResponse
{
    public string PaymentUrl { get; set; } = null!; // required
    public Guid Appointment { get; set; }
}
