using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null);
    Task NavigateBackAsync(Dictionary<string, object>? parameters = null);
}
