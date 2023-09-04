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
using Avalonia;
using Microsoft.JSInterop;
using System.Runtime.InteropServices.JavaScript;

namespace TransportHub.Browser.Services;

public partial class DialogService : IDialogService
{
    private readonly ILogger _logger;

    public DialogService(ILogger logger)
    {
        _logger = logger;
    }

    [JSImport("globalThis.alert")]
    private static partial void Alert([JSMarshalAs<JSType.String>] string message);

    [JSImport("globalThis.confirm")]
    private static partial bool Confirm([JSMarshalAs<JSType.String>] string message);

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

            try
            {
                Alert($"""
                    {title}
                    {message}
                    """);
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
                var result = Confirm($"""
                    {title}
                    {message}
                    """);

                return result ? ButtonResult.Yes : ButtonResult.No;
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
