using AstroForm.Application;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Security;
using AstroForm.Domain.Services;
using AstroForm.Infra;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

[assembly: FunctionsStartup(typeof(AstroForm.Functions.Startup))]
namespace AstroForm.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton<InMemoryFormRepository>();
        services.AddSingleton<IEncryptionService>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var base64 = cfg["EncryptionKey"] ?? Convert.ToBase64String(new byte[16]);
            var key = Convert.FromBase64String(base64);
            return new AesEncryptionService(key);
        });
        services.AddSingleton<IFormRepository>(sp =>
            new EncryptedFormRepository(
                sp.GetRequiredService<InMemoryFormRepository>(),
                sp.GetRequiredService<IEncryptionService>()));
        services.AddSingleton<IActivityLogRepository, InMemoryActivityLogRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IEmailService, InMemoryEmailService>();
        services.AddSingleton<FormPublishService>();
        services.AddSingleton<FormAnswerService>();
        services.AddSingleton<ActivityLogService>();
        services.AddSingleton<UserService>();

        services.AddLogging(logging => logging.AddSerilog());
        services.AddSwaggerGen();
    }
}
