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
using Lindronics.OneOf.Result;

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

    public async Task<bool> ShowConfirmation(string title, string message)
    {
        if (App.Current is null)
            return false;

        var result = await Dispatcher.UIThread.InvokeAsync<ButtonResult>(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.YesNo);


            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return await box.ShowAsync();
            }
            else if (App.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                return await box.ShowAsPopupAsync(singleViewPlatform.MainView as ContentControl);
            }

            return ButtonResult.No;
        });

        return result == ButtonResult.Yes;
    }
}
