using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Core.Assets.Icons;
using TransportHub.Core.Controls;

namespace TransportHub.Core.Markup;

public class TablerIconSourceExtension : MarkupExtension
{
    public TablerIconSourceExtension() { }
    public TablerIconSourceExtension(string icon)
    {
        Icon = icon;
    }
    public double FontSize { get; set; } = 15;
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public string? Icon { get; set; }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (!Tabler.IconsMap.TryGetValue(Icon, out string? glyph))
            return null!;

        return new TablerIconSource()
        {
            FontSize = FontSize,
            FontWeight = FontWeight,
            FontStyle = FontStyle,
            Glyph = glyph,
        };
    }
}
