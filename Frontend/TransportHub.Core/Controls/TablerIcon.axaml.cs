using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TransportHub.Core.Assets.Icons;
using System;
using System.Linq;

namespace TransportHub.Core.Controls;

public partial class TablerIcon : UserControl
{
    public TablerIcon()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<TablerIcon, string?>("Icon", null, false, coerce: Coerce);

    private static string? Coerce(AvaloniaObject @object, string? value)
    {
        if (value is null)
            return null;

        if (Tabler.IconsMap.TryGetValue(value, out string? o))
            return o;

        return value;
    }

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }


    public static readonly StyledProperty<double> SizeProperty =
        TextBlock.FontSizeProperty.AddOwner<TablerIcon>();

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
