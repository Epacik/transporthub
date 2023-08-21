using Avalonia.Controls;
using Lindronics.OneOf.Result;
using System;

namespace TransportHub.Core.Services;

public interface IPageFactory
{
    Result<Control, InvalidOperationException> GetPage(string route);
    Control GetInvalidPage(string message);
}
