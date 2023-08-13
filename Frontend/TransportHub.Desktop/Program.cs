using System;
using Autofac;
using Avalonia;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Sinks.Discord;

namespace TransportHub.Desktop;

static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .ConfigureTransportHub(AddServices)
            .WithInterFont()
            .LogToTrace();

    private static void AddServices(ContainerBuilder builder)
    {
        var options = new SensitiveDataEnricherOptions()
        {
            Mode = MaskingMode.Globally,
        };
        builder.Register<ILogger>(x => new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Discord(1140095240284414072, "mdT1zbS_ZNAXB2sRnQXZS4QLWJTQS93IYcNzAvcbFITK-KnduORzGkFOVB4mMAThxaTZ")
            .WriteTo.File(
                "./Logs/",
                buffered: true,
                flushToDiskInterval: TimeSpan.FromSeconds(5))
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithDemystifiedStackTraces()
            .Enrich.WithSensitiveDataMasking(options)
            .Enrich.FromLogContext()
            .CreateLogger());
    }
}
