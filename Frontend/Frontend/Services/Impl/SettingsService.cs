using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services.Impl;

internal class SettingsService : ISettingsService
{
    private NameValueCollection _settings = ConfigurationManager.AppSettings;

    private bool HasKey(string key) => _settings.AllKeys.Contains(key);

    public string Read(string path, string defaultValue)
    {
        if (HasKey(path))
        {
            return _settings[path] as string ?? defaultValue;
        }
        return defaultValue;
    }

    public bool Read(string path, bool defaultValue)
    {
        return bool.TryParse(Read(path, defaultValue.ToString()), out bool value)
            ? value : defaultValue;
    }

    public int Read(string path, int defaultValue)
    {
        return int.TryParse(Read(path, defaultValue.ToString()), out int value)
            ? value : defaultValue;
    }

    public void Write(string path, string value)
    {
        _settings[path] = value;
    }

    public void Write(string path, bool value)
        => Write(path, value.ToString());

    public void Write(string path, int value)
        => Write(path, value.ToString());
}
