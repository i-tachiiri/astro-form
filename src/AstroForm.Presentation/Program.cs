using Serilog;
using AstroForm.Domain;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Application starting");
    var greeter = new Greeter();
    Log.Information(greeter.Greet("World"));
}
finally
{
    Log.CloseAndFlush();
}
