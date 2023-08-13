using TransportHub.Extensions;
using TransportHub.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransportHub.Services.Impl;

internal class SettingsService : ISettingsService
{

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
        var key = path.ToString();
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
        var settings = configFile.AppSettings.Settings;
        if (HasKey(settings, key))
        {
            return settings[key]?.Value as string ?? defaultValue;
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
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var settings = configFile.AppSettings.Settings;

        var key = path.ToString();
        if (settings[key] == null)
        {
            settings.Add(key, value);
        }
        else
        {
            settings[key].Value = value;
        }

        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
    }

    public void Write(Settings path, bool value)
        => Write(path, value.ToString());

    public void Write(Settings path, int value)
        => Write(path, value.ToString());

    
}
