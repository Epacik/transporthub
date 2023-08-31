using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core;

internal interface INavigationAware
{
    Task OnNavigatedTo(Dictionary<string, object>? parameters = null);
    Task OnNavigatedFrom();
}
