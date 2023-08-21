using System.Threading.Tasks;
using Avalonia.Threading;
using Serilog;
using Serilog.Events;
using TransportHub.Services;
using Android.App;
using System.Threading;

namespace TransportHub.Android.Services;

internal class DialogService : IDialogService
{
    private readonly ILogger _logger;

    public DialogService(ILogger logger)
    {
        _logger = logger;
    }

    private readonly Semaphore _alertSemaphore = new(0, 1);
    public Task ShowAlertAsync(string title, string message)
    {
        if (Avalonia.Application.Current is null)
            return Task.CompletedTask;

        return Task.Run(() =>
        {
            if (_logger.IsEnabled(LogEventLevel.Information))
            {
                _logger.Information("Opening an alert {Title}, {Message}", title, message);
            }

            var activity = MainActivity.Current;

            var builder = new AlertDialog.Builder(activity, AlertDialog.ThemeDeviceDefaultLight);

            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("Ok", (s, e) =>
            {
                _alertSemaphore.Release();
                _logger.Verbose("Alert closed");
            });

            builder.SetCancelable(false);
            

            Dispatcher.UIThread.Invoke(() =>
            {
                var dialog = builder.Create();
                dialog?.Show();
            });

            _alertSemaphore.WaitOne();
        });
    }

    public Task<bool> ShowConfirmation(string title, string message)
    {
        if (Avalonia.Application.Current is null)
            return Task.FromResult(false);

        return Task.Run(() =>
        {
            if (_logger.IsEnabled(LogEventLevel.Information))
            {
                _logger.Information("Opening an alert {Title}, {Message}", title, message);
            }

            var activity = MainActivity.Current;

            bool result = false;


            var builder = new AlertDialog.Builder(activity, AlertDialog.ThemeDeviceDefaultLight);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("Tak", (s, e) =>
            {
                result = true;
                _alertSemaphore.Release();
                _logger.Verbose("Confirmation alert closed (Yes)");
            });
            builder.SetNegativeButton("Nie", (s, e) =>
            {
                _alertSemaphore.Release();
                _logger.Verbose("Confirmation alert closed (No)");
            });

            builder.SetCancelable(false);


            Dispatcher.UIThread.Invoke(() =>
            {
                var dialog = builder.Create();
                dialog?.Show();
            });

            _alertSemaphore.WaitOne();
            return result;
        });
    }

}
