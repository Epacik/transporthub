using Android.App;
using Android.Content.PM;
using Autofac;
using Avalonia;
using Avalonia.Android;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Sinks.Discord;
using System.Configuration;
using System;
using TransportHub.Android.Services;
using TransportHub.Services;
using TransportHub.Common;
using Android.Views;
using Android.Graphics;
using AndroidX.Core.View;
using TransportHub.Core;

namespace TransportHub.Android;

[Activity(
    Label = "TransportHub",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    public MainActivity()
    {
        Current = this;
        this.Window?.SetSoftInputMode(SoftInput.AdjustResize);
        Window?.SetStatusBarColor(Color.White);
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .ConfigureTransportHub(AddServices, IsDemoMode)
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

        builder.RegisterType<DialogService>().As<IDialogService>();
        builder.RegisterType<SettingsService>().As<ISettingsService>();
        builder.Register<IOnScreenKeyboardService>(x =>
        {
            var rootView = this.Window!.DecorView!.RootView;
            var listener = new LayoutListener(rootView!);
            return new OnScreenKayboardService(listener);
        });
    }

    private static bool IsDemoMode => new SettingsService().ReadBool(Settings.DemoMode);

    public static MainActivity? Current { get; private set; }
}
