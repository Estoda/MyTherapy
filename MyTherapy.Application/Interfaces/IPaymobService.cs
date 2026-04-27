using MyTherapy.Application.DTOs.Auth;

namespace MyTherapy.Application.Interfaces;

public interface IPaymobService
{
    Task<string> InitiatePaymentAsync(Guid appointmentId,
                                      decimal amount,
                                      string customerEmail,
                                      string customerName);
}
