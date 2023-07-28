using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Services;

internal interface ISettingsService
{
    string Read(string path, string defaultValue);
    void Write(string path, string value);
    bool Read(string path, bool defaultValue);
    void Write(string path, bool value);
    int Read(string path, int defaultValue);
    void Write(string path, int value);
}
