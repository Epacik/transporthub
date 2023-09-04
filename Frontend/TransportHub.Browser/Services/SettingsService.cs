using TransportHub.Core.Extensions;
using TransportHub.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Services;
using System.Runtime.InteropServices.JavaScript;
using Serilog;

namespace TransportHub.Browser.Services;

internal partial class SettingsService : ISettingsService
{
    private readonly ILogger _logger;

    private static partial class LocalStorage
    {
        [JSImport("globalThis.localStorage.getItem")]
        internal static partial string? GetItem([JSMarshalAs<JSType.String>] string key);

        [JSImport("globalThis.localStorage.setItem")]
        internal static partial void SetItem([JSMarshalAs<JSType.String>] string key, [JSMarshalAs<JSType.String>] string? value);
    }

    public SettingsService(ILogger logger)
    {
        _logger = logger;
    }

    public T? Read<T>(Settings setting)
        where T : ISpanParsable<T>
    {
        var defaultValue = setting.DefaultValue<T>();
        var defaultValueStr = defaultValue?.ToString();

        var value = Read(setting, defaultValueStr);

        if (T.TryParse(value, CultureInfo.InvariantCulture, out T? result))
        {
            return result;
        }

        return defaultValue;
    }

    public string? Read(Settings setting)
    {
        var defaultValue = setting.DefaultValue<string>();

        return Read(setting, defaultValue);
    }

    public bool ReadBool(Settings setting)
    {
        var defaultValue = setting.DefaultValue<bool>();

        var value = Read(setting, defaultValue.ToString());
        if (bool.TryParse(value, out bool b))
            return b;

        return defaultValue;
    }

    public string? Read(Settings path, string? defaultValue)
    {
        var key = $"transporthub.settings.{path}";
        try
        {
            var value = LocalStorage.GetItem(key);
            return value ?? defaultValue;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error reading settings");
        }
        return defaultValue;
    }

    public bool Read(Settings path, bool defaultValue)
    {
        return bool.TryParse(Read(path, defaultValue.ToString()), out bool value)
            ? value : defaultValue;
    }

    public int Read(Settings path, int defaultValue)
    {
        return int.TryParse(Read(path, defaultValue.ToString()), out int value)
            ? value : defaultValue;
    }

    public void Write(Settings path, string? value)
    {
        var key = $"transporthub.settings.{path}";
        try
        {
            LocalStorage.SetItem(key, value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error reading settings");
        }
    }

    public void Write(Settings path, bool value)
        => Write(path, value.ToString());

    public void Write(Settings path, int value)
        => Write(path, value.ToString());
}
