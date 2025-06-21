using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using AstroForm.Infra;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Domain.Tests;

public class SmtpEmailServiceTests
{
    [Fact]
    public async Task SendHtmlEmail_WritesPickupFile()
    {
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(temp);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Smtp:Host", "localhost" },
                { "Smtp:EnableSsl", "false" },
                { "Smtp:From", "from@example.com" }
            })
            .Build();
        var client = new SmtpClient("localhost", 25)
        {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = temp
        };
        var service = new SmtpEmailService(config, client);
        await service.SendHtmlEmailAsync("to@example.com", "Sub", "<b>body</b>");
        var files = Directory.GetFiles(temp);
        Assert.Single(files);
        var text = File.ReadAllText(files[0]);
        Assert.Contains("Subject: Sub", text);
        Assert.Contains("to@example.com", text);
    }
}
