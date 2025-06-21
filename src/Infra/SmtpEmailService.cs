using System.Net.Mail;
using System.Threading.Tasks;
using AstroForm.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace AstroForm.Infra;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpClient _client;
    private readonly string _from;

    public SmtpEmailService(IConfiguration config)
    {
        var host = config["Smtp:Host"] ?? throw new InvalidOperationException("Smtp:Host not configured");
        var portStr = config["Smtp:Port"];
        var user = config["Smtp:Username"];
        var pass = config["Smtp:Password"];
        var enableSsl = config.GetValue("Smtp:EnableSsl", true);
        _from = config["Smtp:From"] ?? user ?? "noreply@example.com";

        var port = 25;
        if (int.TryParse(portStr, out var p)) port = p;
        _client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl
        };
        if (!string.IsNullOrEmpty(user))
        {
            _client.Credentials = new System.Net.NetworkCredential(user, pass);
        }
    }

    public Task SendHtmlEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new MailMessage(_from, to, subject, htmlBody)
        {
            IsBodyHtml = true
        };
        return _client.SendMailAsync(message);
    }
}
