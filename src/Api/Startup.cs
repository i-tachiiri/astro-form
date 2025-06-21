using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AstroForm.Domain;
using AstroForm.Api.Services;

[assembly: FunctionsStartup(typeof(AstroForm.Api.Startup))]
namespace AstroForm.Api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AstroForm API", Version = "v1" });
        });

        var config = builder.GetContext().Configuration;
        var keyBase64 = config["ENCRYPTION_KEY"] ?? Convert.ToBase64String(new byte[32]);
        var ivBase64 = config["ENCRYPTION_IV"] ?? Convert.ToBase64String(new byte[16]);
        var key = Convert.FromBase64String(keyBase64);
        var iv = Convert.FromBase64String(ivBase64);
        builder.Services.AddSingleton<IEncryptionService>(_ => new AesEncryptionService(key, iv));

        builder.Services.AddSingleton<IExternalIdUserService, ExternalIdUserService>();
    }
}
