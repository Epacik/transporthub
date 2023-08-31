using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportHub.Core.AttachedProperties;
using TransportHub.Core.Services;

namespace TransportHub.Core.Views;

public partial class AdministerView : UserControl, INavigationAware
{
    private readonly ILoggedInUserService _loggedInUserService;

    public AdministerView(ILoggedInUserService loggedInUserService)
    {
        InitializeComponent();
        _loggedInUserService = loggedInUserService;
    }

    public Task OnNavigatedFrom()
    {
        return Task.CompletedTask;
    }

    public Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        var current = _loggedInUserService?.User?.UserType;

        bool firstOneSelected = false;
        int i = 0;

        foreach (TabItem? tab in Tabs.Items)
        {
            if (tab is null)
                continue;
            var min = Restriction.GetMinimalUserType(tab);

            var show = min is Api.UserType m && current is Api.UserType c ? c >= m : true;

            tab.IsVisible = show;
            tab.IsEnabled = show;

            if (show && !firstOneSelected)
            {
                Tabs.SelectedIndex = i;
                firstOneSelected = true;
            }

            i++;
        }

        return Task.CompletedTask;
    }
}
