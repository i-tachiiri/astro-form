using Microsoft.Extensions.Hosting;
using Xunit;
using Serilog;

namespace Api.Tests;

public class SmokeTests
{
    [Fact]
    public void HostBuild_DoesNotThrow()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .UseSerilog((ctx, cfg) => cfg.WriteTo.Console())
            .Build();
        host.Dispose();
    }
}
