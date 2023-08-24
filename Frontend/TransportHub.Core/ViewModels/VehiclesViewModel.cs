using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.ViewModels;

public class VehiclesViewModel : ObservableObject, INavigationAwareViewModel
{
    public Task OnNavigatedFrom()
    {
        return Task.CompletedTask;
    }

    public Task OnNavigatedTo(Dictionary<string, object>? parameters = null)
    {
        return Task.CompletedTask;
    }
}
