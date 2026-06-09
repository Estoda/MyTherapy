using MyTherapy.Application.DTOs.Auth;

namespace MyTherapy.Application.Interfaces;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string toEmail, string code);
}
