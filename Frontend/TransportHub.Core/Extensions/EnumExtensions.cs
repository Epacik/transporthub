using TransportHub.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Extensions;

internal static class EnumExtensions
{
    public static T? DefaultValue<T>(this Settings setting)
    {
        var type = setting.GetType();
        var memInfo = type.GetMember(setting.ToString());
        var attribute = (DefaultValueAttribute?)memInfo[0].GetCustomAttribute(typeof(DefaultValueAttribute), false);

        if (attribute?.Value is T value)
        {
            return value;
        }

        return default;
    }
}
