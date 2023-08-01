using System.Collections.Generic;

namespace Frontend.Helpers;

internal static class Settings
{
    public const string IpAddress = nameof(IpAddress);
    public const string DemoMode = nameof(DemoMode);

    static Dictionary<string, string> a = new()
    {
        { "key", "value" },
    };
}
