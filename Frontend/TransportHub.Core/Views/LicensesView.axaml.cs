using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TransportHub.Core.DesignHelpers;

namespace TransportHub.Core.Views;

public partial class LicensesView : UserControl
{
    public LicensesView()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
        {
            DataContext = DesignViewModels.LicensesViewModel;
        }
    }
}
