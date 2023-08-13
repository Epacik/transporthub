using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message);
    Task<bool> ShowConfirmation(string title, string message);
}
