using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub;

internal interface INavigationAwareViewModel
{
    Task OnNavigatedTo(Dictionary<string, object>? parameters = null);
    Task OnNavigatedFrom();
}
