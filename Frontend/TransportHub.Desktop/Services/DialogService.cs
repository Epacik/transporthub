using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Threading;
using Lindronics.OneOf.Result;
using Serilog;
using Serilog.Events;
using TransportHub.Services;

namespace TransportHub.Desktop.Services;

public class DialogService : IDialogService
{
    private readonly ILogger _logger;

    public DialogService(ILogger logger)
    {
        _logger = logger;
    }
    public async Task ShowAlertAsync(string title, string message)
    {
        if (Avalonia.Application.Current is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_logger.IsEnabled(LogEventLevel.Information))
            {
                _logger.Information("Opening an alert {Title}, {Message}", title, message);
            }

            var box = MessageBoxManager
                .GetMessageBoxStandard(title, message, ButtonEnum.Ok);

            try
            {
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    await box.ShowAsync();
                }
                else if (Avalonia.Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                {
                    await box.ShowAsync();
                    //await box.ShowAsPopupAsync(singleViewPlatform.MainView as ContentControl);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured");
                throw;
            }
        });
    }

    public async Task<bool> ShowConfirmation(string title, string message)
    {
        if (Avalonia.Application.Current is null)
            return false;

        if (_logger.IsEnabled(LogEventLevel.Information))
        {
            _logger.Information("Opening a confirmation alert {Title}, {Message}", title, message);
        }

        var result = await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.YesNo);


            try
            {
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    return await box.ShowAsync();
                }
                else if (Avalonia.Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                {
                    return await box.ShowAsPopupAsync(singleViewPlatform.MainView as ContentControl);
                }

                return ButtonResult.No;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured");
                throw;
            }
        });

        return result == ButtonResult.Yes;
    }
}
