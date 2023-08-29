using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services.Impl;

public class RefreshUserDataService : IRefreshUserDataService
{
    public event Action? Refresh;

    public void RequestRefresh()
    {
        Refresh?.Invoke();
    }
}
