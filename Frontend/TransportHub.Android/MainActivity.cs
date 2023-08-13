using Android.App;
using Android.Content.PM;
using Autofac;
using Avalonia;
using Avalonia.Android;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Sinks.Discord;

namespace TransportHub.Android;

[Activity(
    Label = "TransportHub",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .ConfigureTransportHub(AddServices)
            .WithInterFont();
    }

    private void AddServices(ContainerBuilder builder)
    {
        var options = new SensitiveDataEnricherOptions()
        {
            Mode = MaskingMode.Globally,
        };


        builder.Register<ILogger>(x => new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Discord(1140234047856070687, "aN6iDaIpp-G2zSBZG0H4_KC9dyh1XAVh38bNECTKeVCXJUp7CW0blovuacf4E291bjjb")
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
