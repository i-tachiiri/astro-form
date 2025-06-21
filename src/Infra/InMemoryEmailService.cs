using System.Collections.Generic;
using System.Threading.Tasks;
using AstroForm.Domain.Services;

namespace AstroForm.Infra
{
    public class InMemoryEmailService : IEmailService
    {
        public List<EmailMessage> Messages { get; } = new();

        public Task SendHtmlEmailAsync(string to, string subject, string htmlBody)
        {
            Messages.Add(new EmailMessage(to, subject, htmlBody));
            return Task.CompletedTask;
        }
    }

    public record EmailMessage(string To, string Subject, string HtmlBody);
}
