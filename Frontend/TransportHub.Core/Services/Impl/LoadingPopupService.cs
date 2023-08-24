using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services.Impl;

public class LoadingPopupService : ILoadingPopupService
{
    public async Task Hide()
    {
        var app = App.Current as App;
        if (app is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(app.HideLoadingPopup);
    }

    public async Task Show(string message)
    {
        var app = App.Current as App;
        if (app is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(() => app.ShowLoadingPopup(message));
    }
}
