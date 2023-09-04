using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services;

public interface IBackgroundWorker
{
    Task Run(Action action);
}
