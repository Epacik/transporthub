using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services.Impl;

internal class SystemInfoService : ISystemInfoService
{
    public bool IsDesktop =>
        OperatingSystem.IsWindows() ||
        OperatingSystem.IsFreeBSD() ||
        OperatingSystem.IsMacCatalyst() ||
        OperatingSystem.IsMacOS() ||
        OperatingSystem.IsLinux();

    public bool IsMobile =>
        OperatingSystem.IsAndroid() ||
        OperatingSystem.IsIOS();

    public bool IsBrowser => OperatingSystem.IsBrowser();
}
