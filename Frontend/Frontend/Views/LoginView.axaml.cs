using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace Frontend.Views;

public partial class LoginView : UserControl
{
    private Thickness _originalHeaderMargin;

    public LoginView()
    {
        InitializeComponent();
    }
}
