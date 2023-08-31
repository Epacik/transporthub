using System.ComponentModel;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;
using TransportHub.Core.Models;

namespace TransportHub.Core.Services;

public interface ILoggedInUserService : INotifyPropertyChanged
{
    Task ForceRefreshAsync();
    UserModel? User { get; }
}
