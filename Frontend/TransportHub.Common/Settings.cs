using System.Collections.Generic;
using System.ComponentModel;

namespace TransportHub.Common;

public enum Settings
{
    [DefaultValue("127.0.0.1:8080")]
    IpAddress,
    [DefaultValue(false)]
    DemoMode,
}
