using Microsoft.Extensions.Hosting;
using Serilog;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .UseSerilog((ctx, cfg) => cfg.WriteTo.Console())
    .Build();

host.Run();
