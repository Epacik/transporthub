using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Resources;

namespace Frontend.Views;

public partial class AboutView : UserControl
{
    //private readonly ResourceManager _resourceManager;

    public AboutView()
    {
        InitializeComponent();
       // _resourceManager = new ResourceManager("Frontend.Resources.Licenses", typeof(AboutView).Assembly);

    }
}
