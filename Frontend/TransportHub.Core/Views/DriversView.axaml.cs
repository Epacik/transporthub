using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TransportHub.Core.DesignHelpers;

namespace TransportHub.Core.Views;

public partial class DriversView : UserControl
{
    public DriversView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = DesignViewModels.DriversViewModel;
        }
    }
}
