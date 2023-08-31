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
using TransportHub.Services.Impl;
using TransportHub.Core.Services;
using TransportHub.Core.Services.InMemory.API;
using Avalonia.Input.Platform;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

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
        builder.RegisterType<UsersService>().As<IUsersService>();
        builder.RegisterType<LicenseTypeService>().As<ILicenseTypeService>();
        builder.RegisterType<VehicleService>().As<IVehicleService>();
        builder.RegisterType<DriverService>().As<IDriverService>();
        builder.RegisterType<DriversLicenseService>().As<IDriversLicenseService>();

        return builder.LoadCommonServices();
    }

    public static ContainerBuilder LoadMockServices(this ContainerBuilder builder)
    {
        builder.RegisterType<InMemoryAuthorizationService>().As<IAuthorizationService>().SingleInstance();
        builder.RegisterType<InMemoryUsersService>().As<IUsersService>();

        return builder.LoadCommonServices();
    }

    public static ContainerBuilder LoadCommonServices(this ContainerBuilder builder)
    {
        builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>();
        builder.RegisterType<PageFactory>().As<IPageFactory>();
        builder.RegisterType<NavigationService>().As<INavigationService>();
        builder.RegisterType<SystemInfoService>().As<ISystemInfoService>();
        builder.RegisterType<Serializer>().As<IJsonSerializer>();
        builder.RegisterType<LoadingPopupService>().As<ILoadingPopupService>();
        builder.RegisterType<ReportErrorService>().As<IReportErrorService>();
        builder.RegisterType<ClipboardProvider>().As<IClipboardProvider>();
        builder.RegisterType<UserProvidedImageService>().As<IUserProvidedImageService>();
        builder.RegisterType<LoggedInUserService>().As<ILoggedInUserService>().SingleInstance();
        builder.Register(c =>
        {
            var lifetime = Application.Current?.ApplicationLifetime;
            if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow?.StorageProvider;

            else if (lifetime is ISingleViewApplicationLifetime singleView)
                return TopLevel.GetTopLevel(singleView.MainView)?.StorageProvider;

            return null;
        });

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
        builder.RegisterViewModelBinding<AdministerView, AdministerViewModel>(Routes.Administer);
        builder.RegisterViewModelBinding<UsersView, UsersViewModel>(Routes.Users);
        builder.RegisterViewModelBinding<DriversView, DriversViewModel>(Routes.Drivers);
        builder.RegisterViewModelBinding<VehiclesView, VehiclesViewModel>(Routes.Vehicles);
        builder.RegisterViewModelBinding<ClientsView, ClientsViewModel>(Routes.Clients);
        builder.RegisterViewModelBinding<LicensesView, LicensesViewModel>(Routes.Licenses);

        return builder;
    }
}
