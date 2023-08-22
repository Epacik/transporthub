using System.Collections.Generic;
using System.ComponentModel;

namespace TransportHub.Common;

public enum Settings
{
    [DefaultValue(DefaultValues.ServerAddress)]
    IpAddress,
    [DefaultValue(false)]
    DemoMode,
}
