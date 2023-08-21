using System;
using TransportHub.Services;

namespace TransportHub.Core.Services.Impl;

public class OnScreenKeyboardServiceDefaultImpl : IOnScreenKeyboardService
{
    public double CurrentHeight => 0;

    public event Action<double>? HeightChanged;
}
