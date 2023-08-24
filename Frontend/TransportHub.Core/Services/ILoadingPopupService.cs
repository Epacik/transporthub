using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services;

public interface ILoadingPopupService
{
    public Task Show(string message);
    public Task Hide();
}
