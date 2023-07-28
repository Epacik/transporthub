using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Frontend.ViewModels;
using System;

namespace Frontend.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        nvSample.BackRequested += NvSample_BackRequested;
    }

    private async void NvSample_BackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
    {
        if (DataContext is not MainViewModel model)
            return;

        await model.NavigateBackAsync(null);
    }
}
