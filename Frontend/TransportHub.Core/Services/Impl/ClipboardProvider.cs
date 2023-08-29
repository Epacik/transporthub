using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia;

namespace TransportHub.Core.Services.Impl;

public class ClipboardProvider : IClipboardProvider
{
    public IClipboard? Get()
    {
        var lifetime = Application.Current?.ApplicationLifetime;
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow?.Clipboard;

        else if (lifetime is ISingleViewApplicationLifetime singleView)
            return TopLevel.GetTopLevel(singleView.MainView)?.Clipboard;

        return null;
    }
}
