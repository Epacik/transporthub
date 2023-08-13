using Autofac;
using Avalonia;
using TransportHub.Extensions;
using TransportHub.ViewModels;
using TransportHub.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Common;
using System.Net.Http;
using TransportHub.Services.Impl;
using TransportHub.Services;
using FluentAvalonia.UI.Controls;
using System.Configuration;
using TransportHub.Services.InMemory.API;
using TransportHub.Api;
using TransportHub.Api.Impl;

namespace TransportHub;

public static class BuildAppExtensions
{
    public static AppBuilder ConfigureTransportHub(
        this AppBuilder builder,
        Action<ContainerBuilder>? addServices = null)
    {
        builder.ConfigureAutofac(addServices);
        return builder;
    }

    public static AppBuilder ConfigureAutofac(this AppBuilder builder, Action<ContainerBuilder>? addServices)
    {
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
        var settings = configFile.AppSettings.Settings;
        var key = Settings.DemoMode.ToString();

        

        var containerBuilder = new ContainerBuilder();

        if (settings.AllKeys.Contains(key) &&
            string.Equals(settings[key].Value, bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
            containerBuilder = containerBuilder.LoadMockServices();
        }
        else
        {
            containerBuilder = containerBuilder.LoadServices();
        }

        addServices?.Invoke(containerBuilder);


        var container = containerBuilder
            .LoadViews()
            .Build();

        App.Container = container;
        return builder;
    }

    public static ContainerBuilder LoadServices(this ContainerBuilder builder)
    {
        builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().SingleInstance();

        return builder.LoadCommonServices();
    }

    public static ContainerBuilder LoadMockServices(this ContainerBuilder builder)
    {
        builder.RegisterType<InMemoryAuthorizationService>().As<IAuthorizationService>().SingleInstance();

        return builder.LoadCommonServices();
    }

    public static ContainerBuilder LoadCommonServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SettingsService>().As<ISettingsService>();
        builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
        builder.RegisterType<PageFactory>().As<IPageFactory>();
        builder.RegisterType<DialogService>().As<IDialogService>();
        builder.RegisterType<NavigationService>().As<INavigationService>();
        builder.RegisterType<SystemInfoService>().As<ISystemInfoService>();

        return builder;
    }


    public static ContainerBuilder LoadViews(this ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MainView>().BindToViewModel<MainView, MainViewModel>().SingleInstance();
        builder.RegisterViewModelBinding<LoginView, LoginViewModel>(Routes.Login);
        builder.RegisterViewModelBinding<DashboardView, DashboardViewModel>(Routes.Dashboard);
        builder.RegisterViewModelBinding<StartupSettingsView, StartupSettingsViewModel>(Routes.StartupSettings);
        return builder;
    }
}
