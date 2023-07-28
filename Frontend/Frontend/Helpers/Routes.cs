using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Helpers;

internal static class Routes
{
    public const string Login = $"//{nameof(Login)}";
    public const string StartupSettings = $"{Login}/{nameof(StartupSettings)}";

    public const string Dashboard = $"//{nameof(Dashboard)}";
}
