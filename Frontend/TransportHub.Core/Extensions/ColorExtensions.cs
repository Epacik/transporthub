using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Extensions;

internal static class ColorExtensions
{

    public static Color WithOpacity(this Color color, float opacity)
        => color.WithOpacity((byte)(255 * opacity));
    public static Color WithOpacity(this Color color, byte opacity)
        => new(opacity, color.R, color.G, color.B);
    public static ISolidColorBrush WithOpacity(this ISolidColorBrush brush, float opacity)
        => brush.WithOpacity((byte)(255 * opacity));
    public static ISolidColorBrush WithOpacity(this ISolidColorBrush brush, byte opacity)
        => new SolidColorBrush(brush.Color.WithOpacity(opacity));
}
