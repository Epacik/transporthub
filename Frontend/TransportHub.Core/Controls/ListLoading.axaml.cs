using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TransportHub.Core.Controls;

public partial class ListLoading : UserControl
{
    public ListLoading()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<ListLoading, double>(nameof(ItemHeight), defaultValue: 50);

    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<ListLoading, int>(nameof(ItemCount), defaultValue:10, validate: v => v >= 0);

    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        set
        {
            SetValue(ItemCountProperty, value);

            var currentCount = Items.Count;

            if (value > currentCount)
            {
                for(int i = currentCount; i < value; i++)
                {
                    Items.Add(new() { Delay = TimeSpan.FromMilliseconds(100 * (i - currentCount)) } );
                }
            }
            else if (value < currentCount)
            {
                for (int i = currentCount; i > value; i--)
                {
                    Items.Remove(Items.Last());
                }
            }
        }
    }

    public ObservableCollection<ListLoadingItem> Items { get; } = new();
}
public class ListLoadingItem
{
    public TimeSpan Delay { get; set; }
}
