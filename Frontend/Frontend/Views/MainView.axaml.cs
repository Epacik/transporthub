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

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainView()
    {
        InitializeComponent();
        nvSample.BackRequested += NvSample_BackRequested;
        //nvSample.Header

        if (ViewModel is null)
            return;

        ViewModel.HeaderChanged += ViewModel_HeaderChanged;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (ViewModel is null)
            return;

        ViewModel.HeaderChanged += ViewModel_HeaderChanged;
        UpdateHeader();
    }

    private void ViewModel_HeaderChanged(object? sender, EventArgs e)
    {
        UpdateHeader();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateHeader();
    }

    void UpdateHeader()
    {
        var header = this.FindNameScope()?.Find<ContentControl>("HeaderContent");
    }

    private async void NvSample_BackRequested(object? sender, NavigationViewBackRequestedEventArgs e)
    {
        if (ViewModel is null)
            return;

        await ViewModel.NavigateBackAsync(null);
    }
}
