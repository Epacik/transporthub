using Frontend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services;

internal interface ISettingsService
{
    string? Read(Settings setting);
    bool ReadBool(Settings settings);
    T? Read<T>(Settings setting) where T : ISpanParsable<T>;
    string? Read(Settings path, string defaultValue);
    void Write(Settings path, string? value);
    bool Read(Settings path, bool defaultValue);
    void Write(Settings path, bool value);
    int Read(Settings path, int defaultValue);
    void Write(Settings path, int value);
}
