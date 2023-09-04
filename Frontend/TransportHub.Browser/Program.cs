using System.Configuration;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Autofac;
using Avalonia;
using Avalonia.Browser;
using Serilog.Enrichers.Sensitive;
using Serilog;
using TransportHub;
using TransportHub.Common;
using TransportHub.Core;
using TransportHub.Core.Services.Impl;
using TransportHub.Services;
using Serilog.Sinks.Discord;
using Serilog.Exceptions;
using TransportHub.Browser.Services;
using System.Runtime.InteropServices.JavaScript;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
    private static async Task Main(string[] args)
        => await BuildAvaloniaApp()
            .ConfigureTransportHub(AddServices)
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

    private static void AddServices(ContainerBuilder builder)
    {
        var options = new SensitiveDataEnricherOptions()
        {
            Mode = MaskingMode.Globally,
        };
        builder.Register<ILogger>(x => new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Discord(1140095240284414072, "usnrVxfvCAH22E8TH_w3Uc40rcC51sSyHQMwyIa9uHHbppXlLFaXglcI8M5w4VImSTRf")
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

        builder.RegisterType<DialogService>().As<IDialogService>();
        builder.RegisterType<SettingsService>().As<ISettingsService>();
        builder.RegisterType<OnScreenKeyboardServiceDefaultImpl>().As<IOnScreenKeyboardService>();
        builder.RegisterType<BackgroundWorker>().As<IBackgroundWorker>().SingleInstance();
    }

    private static bool IsDemoMode
    {
        get
        {
            var value = GetItem("transporthub.settings.DemoMode");
            return value?.ToLowerInvariant() == "true";
        }
    }

    [JSImport("globalThis.localStorage.getItem")]
    internal static partial string? GetItem([JSMarshalAs<JSType.String>] string key);
}
