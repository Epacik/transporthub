using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using TransportHub.Core.ViewModels;

namespace TransportHub.Core.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private async void PassBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if(e.Key == Avalonia.Input.Key.Enter && DataContext is LoginViewModel vm)
        {
            await vm.LoginUser();
        }
    }
}
