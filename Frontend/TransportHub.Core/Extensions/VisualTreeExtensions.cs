using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using TransportHub.Core.Exceptions;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Extensions;

internal static class VisualTreeExtensions
{
    public static Result<TControl?, Exception> FindVisualDescendant<TControl>(
        this Visual visual,
        string? name = null)
        where TControl : Visual
    {
        var children = visual.GetVisualChildren().ToArray();
        if (children.Length == 0)
        {
            return new ItemNotFoundException();
        }

        var item = children.FirstOrDefault(
            x => x is TControl && (string.IsNullOrEmpty(name) || x.Name == name));

        if (item is not null)
        {
            return item as TControl;
        }

        List<Exception> exceptions = new();

        foreach (var child in children)
        {
            var result = child.FindVisualDescendant<TControl>(name);
            if (result is null)
            {
                continue;
            }

            if (result.IsError)
            {
                exceptions.Add(result.UnwrapErr());
                continue;
            }

            return result;
        }

        return new AggregateException(exceptions);
    }

    public static Result<IEnumerable<TControl>, Exception> FindVisualDescendants<TControl>(
        this Visual visual,
        string? name = null)
        where TControl : Visual
    {
        try
        {
            var descendants = new List<TControl>();

            var children = visual.GetVisualChildren().ToArray();
            if (children.Length == 0)
            {
                return Array.Empty<TControl>();
            }

            var items = children.Where(
                x => x is TControl && (string.IsNullOrEmpty(name) || x.Name == name));

            if (items is not null)
            {
                descendants.AddRange(items.Cast<TControl>());
            }

            List<Exception> exceptions = new();

            foreach (var child in children)
            {
                var result = child.FindVisualDescendants<TControl>(name);
                if (result is null)
                {
                    continue;
                }

                if (result.IsError)
                {
                    exceptions.Add(result.UnwrapErr());
                    continue;
                }

                descendants.AddRange(result.Unwrap());
            }

            if (exceptions.Count > 0)
            {
                return new AggregateException(exceptions);
            }

            return descendants;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
