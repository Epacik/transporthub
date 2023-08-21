using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TransportHub.Core.ViewModels;
using TransportHub.Core.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace TransportHub.Core;

public partial class App : Application, IDisposable
{
    internal static IContainer? Container { get; set; }
    public ILifetimeScope? ContainerLifetime { get; private set; }
    

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ContainerLifetime = Container?.BeginLifetimeScope();
        var mainView = ContainerLifetime?.Resolve<MainView>();
        _mainViewModel = mainView!.DataContext as MainViewModel;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = mainView,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = mainView;
        }
        base.OnFrameworkInitializationCompleted();
    }

    bool _disposed = false;
    private MainView? _navigationView;
    private MainViewModel? _mainViewModel;

    public void Dispose()
    {
        _disposed = true;
        ContainerLifetime?.Dispose();
        Container?.Dispose();
    }

    public Task NavigateBackAsync(Dictionary<string, object>? parameters = null)
    {
        return _mainViewModel?.NavigateBackAsync(parameters) ?? Task.FromResult(0);
    }

    public Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
    {
        return _mainViewModel?.NavigateToAsync(route, parameters) ?? Task.FromResult(0);
    }
    private static bool IsProduction()
    {
#if DEBUG
        return false;
#else
        return true;
#endif
    }


    ~App()
    {
        if (!_disposed)
            Dispose();
    }
}
