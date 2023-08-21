using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using TransportHub.Common;
using TransportHub.Services;
using TransportHub.Core.Extensions;
using Android.Preferences;
using Android.App;
using Android.Content;

namespace TransportHub.Android.Services;


internal class SettingsService : ISettingsService
{

    private static string _sharedName = "TransportHub.Settings";
    private ISharedPreferences? GetPreferences()
        => Application.Context.GetSharedPreferences(_sharedName, FileCreationMode.Private);

    private bool HasKey(KeyValueConfigurationCollection settings, string key) => settings.AllKeys.Contains(key);
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
        using var pref = GetPreferences();

        return pref?.GetString(path.ToString(), defaultValue);
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
        using var pref = GetPreferences();
        using var edit = pref?.Edit();

        if (edit is null)
            return;

        edit.PutString(path.ToString(), value);
        edit.Apply();
    }

    public void Write(Settings path, bool value)
        => Write(path, value.ToString());

    public void Write(Settings path, int value)
        => Write(path, value.ToString());


}
