using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Frontend.Views;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Frontend.Services.Impl;

internal class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message)
    {
        if (App.Current is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {

            var box = MessageBoxManager
                .GetMessageBoxStandard(title, message, ButtonEnum.Ok);

            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await box.ShowAsync();
            }
            else if (App.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                await box.ShowAsPopupAsync(singleViewPlatform.MainView as ContentControl);
            }
        });
    }
}
