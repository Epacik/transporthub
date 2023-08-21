using FluentAvalonia.UI.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Controls;

public class TablerIconSource : FontIconSource
{
    public TablerIconSource()
    {
        FontFamily = FontFamily.Parse(
            "#tabler-icons",
            new Uri("avares://TransportHub.Core/Assets/Icons/tabler/tabler-icons.ttf"));
    }
}
