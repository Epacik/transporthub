using Frontend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services.Impl;

internal class NavigationService : INavigationService
{
    private App? _app;

    public NavigationService()
    {
        _app = App.Current as App;
    }
    public Task NavigateBackAsync(Dictionary<string, object>? parameters = null)
    {
        return _app?.NavigateBackAsync(parameters) ?? Task.FromResult(0);
    }

    public Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
    {
        return _app?.NavigateToAsync(route, parameters) ?? Task.FromResult(0);
    }
}
