using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TransportHub.Core.DesignHelpers;

namespace TransportHub.Core.Views;

public partial class VehiclesView : UserControl
{
    public VehiclesView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = DesignViewModels.VehiclesViewModel;
        }
    }
}
