using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Services;

namespace TransportHub.Core.Services.Impl.Empty;

internal class EmptyDialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ShowConfirmation(string title, string message)
    {
        return Task.FromResult(true);
    }
}
