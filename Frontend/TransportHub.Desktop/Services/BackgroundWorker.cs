using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransportHub.Services;

namespace TransportHub.Desktop.Services;

internal class BackgroundWorker : IBackgroundWorker
{
    public async Task Run(Action action)
    {
        await Task.Yield();
        new Thread(() => action())
            .Start();
    }
}
