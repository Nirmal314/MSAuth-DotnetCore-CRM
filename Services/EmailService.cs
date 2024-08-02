using Microsoft.Extensions.Options;
using MSAuth.Interfaces;
using MSAuth.Models;
using System.Net;
using System.Net.Mail;

namespace MSAuth.Services;

public class EmailService : IEmailService
{
    public void SendEmail(string email, string subject, string message)
    {
        SmtpClient client = GetSmtpClient();

        MailMessage mailMessage = new()
        {
            From = new MailAddress("terrill84@ethereal.email"),
            Subject = subject,
            Body = message,
            IsBodyHtml = false
        };
        mailMessage.To.Add(email);

        client.Send(mailMessage);
    }

    private static SmtpClient GetSmtpClient()
    {
        SmtpClient client = new("smtp.ethereal.email", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("terrill84@ethereal.email", "W3t3tPv6nfXU4GAbSX")
        };

        return client;
    }
}
