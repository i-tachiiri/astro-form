using System.Threading.Tasks;

namespace AstroForm.Domain.Services
{
    public interface IEmailService
    {
        Task SendHtmlEmailAsync(string to, string subject, string htmlBody);
    }
}
