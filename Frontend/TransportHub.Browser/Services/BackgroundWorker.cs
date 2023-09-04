using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportHub.Services;

namespace TransportHub.Browser.Services;

internal class BackgroundWorker : IBackgroundWorker
{
    public async Task Run(Action action)
    {
        await Task.Yield();
        _ = Task.Run(() => action());
    }

}
