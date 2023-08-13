using Autofac;
using Avalonia.Controls;
using Avalonia.Layout;
using Lindronics.OneOf.Result;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Services.Impl;

internal class PageFactory : IPageFactory
{
    private readonly ILifetimeScope _scope;

    public PageFactory(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public Control GetInvalidPage(string message)
    {
        return new Border
        {
            Child = new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            },
        };
    }

    public Result<Control, InvalidOperationException> GetPage(string route)
    {
        try
        {
            if (_scope.TryResolveNamed(route, out Control? instance) && instance is not null)
            {
                return instance;
            }
        }
        catch (Exception ex)
        {
            return new InvalidOperationException(ex.Message, ex);
        }

        return new InvalidOperationException($"Page not found (route: {route})");
    }
}
