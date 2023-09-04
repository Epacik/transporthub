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

public class TablerIconExtension : MarkupExtension
{
    public TablerIconExtension() { }
    public TablerIconExtension(string icon)
    {
        Icon = icon;
    }
    public double FontSize { get; set; } = 15;
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public string? Icon { get; set; }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new TablerIcon()
        {
            FontSize = FontSize,
            FontWeight = FontWeight,
            FontStyle = FontStyle,
            Icon = Icon,
        };
    }
}
