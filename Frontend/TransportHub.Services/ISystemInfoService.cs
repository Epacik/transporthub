using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services;

public interface ISystemInfoService
{
    bool IsDesktop { get; }
    bool IsMobile { get; }

    bool IsBrowser { get; }
}
