using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services.Impl.Empty;

internal class EmptyLoadingPopupService : ILoadingPopupService
{
    public Task Hide()
    {
        return Task.CompletedTask;
    }

    public Task Show(string message)
    {
        return Task.CompletedTask;
    }
}
