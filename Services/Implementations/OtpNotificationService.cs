using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Utilities;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class OtpNotificationService : IOtpNotificationService
{
    private readonly SmtpMail _smtpSettings;

    public OtpNotificationService(IOptions<SmtpMail> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public void SendEmailOtp(string email, string otpCode)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _smtpSettings.FromName,
                _smtpSettings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(email));

        message.Subject = "Verify Your RideHailingAPI Email";

        message.Body = new TextPart("html")
        {
            Text = MailUtils.Create(otpCode)
        };

        using var smtp = new SmtpClient();

        smtp.Connect(
            _smtpSettings.Host,
            _smtpSettings.Port,
            SecureSocketOptions.StartTls);

        smtp.Authenticate(
            _smtpSettings.Username,
            _smtpSettings.Password);

        smtp.Send(message);

        smtp.Disconnect(true);
    }

    public void SendPhoneOtp(string phoneNumber, string otpCode)
    {
        throw new NotImplementedException();
    }

    public void SendPasswordResetOtp(string email, string otpCode)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _smtpSettings.FromName,
                _smtpSettings.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(email));

        message.Subject = "Reset Your RideHailingAPI Password";

        message.Body = new TextPart("html")
        {
            Text = MailUtils.PasswordResetOtpTemplate.Create(otpCode)
        };

        using var smtp = new SmtpClient();

        smtp.Connect(
            _smtpSettings.Host,
            _smtpSettings.Port,
            SecureSocketOptions.StartTls);

        smtp.Authenticate(
            _smtpSettings.Username,
            _smtpSettings.Password);

        smtp.Send(message);

        smtp.Disconnect(true);
    }
    
}