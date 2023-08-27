using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using TransportHub.Core.Controls;
using TransportHub.Core.DesignHelpers;

namespace TransportHub.Core.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = DesignViewModels.UsersViewModel;
        }
    }
}
