using Autofac;
using Avalonia;
using TransportHub.Core.Extensions;
using TransportHub.Core.ViewModels;
using TransportHub.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Common;
using System.Net.Http;
using TransportHub.Core.Services.Impl;
using TransportHub.Services;
using FluentAvalonia.UI.Controls;
using System.Configuration;
using TransportHub.Services.InMemory.API;
using TransportHub.Api;
using TransportHub.Api.Impl;
using TransportHub.Core.Views;
using TransportHub.Core.ViewModels;
using TransportHub.Services.Impl;
using TransportHub.Core.Services;

namespace TransportHub.Core;

public static class BuildAppExtensions
{
    public static AppBuilder ConfigureTransportHub(
        this AppBuilder builder,
        Action<ContainerBuilder>? addServices = null,
        bool isDemoMode = false)
    {
        builder.ConfigureAutofac(addServices, isDemoMode);
        return builder;
    }

    public static AppBuilder ConfigureAutofac(
        this AppBuilder builder,
        Action<ContainerBuilder>? addServices,
        bool isDemoMode)
    {

        var containerBuilder = new ContainerBuilder();

        if (isDemoMode)
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
        builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
        builder.RegisterType<PageFactory>().As<IPageFactory>();
        builder.RegisterType<NavigationService>().As<INavigationService>();
        builder.RegisterType<SystemInfoService>().As<ISystemInfoService>();

        return builder;
    }


    public static ContainerBuilder LoadViews(this ContainerBuilder builder)
    {
        builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MainView>().BindToViewModel<MainView, MainViewModel>().SingleInstance();
        builder.RegisterViewModelBinding<LoginView, LoginViewModel>(Routes.Login);
        builder.RegisterViewModelBinding<StartupSettingsView, StartupSettingsViewModel>(Routes.StartupSettings);
        builder.RegisterViewModelBinding<DashboardView, DashboardViewModel>(Routes.Dashboard);
        builder.RegisterViewModelBinding<OrdersView, OrdersViewModel>(Routes.Orders);
        builder.RegisterViewModelBinding<SettingsView, SettingsViewModel>(Routes.Settings);
        builder.RegisterViewModelBinding<UsersView, UsersViewModel>(Routes.Users);
        return builder;
    }
}
