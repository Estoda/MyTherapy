using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MyTherapy.Application.Interfaces;
using MailKit.Net.Smtp;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendVerificationCodeAsync(string toEmail, string code)
    {
        var from = _config["Email:From"]!;
        var password = _config["Email:Password"]!;
        var host = _config["Email:Host"]!;
        var port = int.Parse(_config["Email:Port"]!);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("MyTherapy", from));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Your MyTherapy Verification Code";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <h2>MyTherapy Email Verification</h2>
                <p>Your verification code is:</p>
                <h1 style="letter-spacing: 8px; color: #4A90D9;">{code}</h1>
                <p>This code expires in <strong>10 minutes</strong>.</p>
                <p>If you didn't request this, ignore this email.</p>
            """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(from, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}