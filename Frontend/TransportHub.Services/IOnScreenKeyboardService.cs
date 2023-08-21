using System;

namespace TransportHub.Services;

public interface IOnScreenKeyboardService
{
    double CurrentHeight { get; }
    event Action<double>? HeightChanged;
}
