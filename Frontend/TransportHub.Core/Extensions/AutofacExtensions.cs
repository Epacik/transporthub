using Autofac.Builder;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core.Resolving.Pipeline;
using Avalonia.Controls;

namespace TransportHub.Core.Extensions;

internal static class AutofacExtensions
{
    public static ContainerBuilder RegisterViewModelBinding<TView, TViewModel>(
        this ContainerBuilder builder,
        string? route = null)
        where TView : Control
        where TViewModel : class
    {
        builder.RegisterType<TViewModel>();
        builder.RegisterType<TView>().BindToViewModel<TView, TViewModel>();


        if (!string.IsNullOrWhiteSpace(route))
        {
            builder
                .RegisterType<TView>()
                .Named<Control>(route)
                .BindToViewModel<TView, TViewModel>();
        }

        return builder;
    }

    public static IRegistrationBuilder<TView, ConcreteReflectionActivatorData, SingleRegistrationStyle> BindToViewModel<TView, TViewModel>(this IRegistrationBuilder<TView, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder)
        where TViewModel : class
    {
        registrationBuilder.ConfigurePipeline(p =>
        {
            p.Use(PipelinePhase.Activation, (context, next) =>
            {
                next(context);
                if (context.Instance is Control view)
                {
                    view.DataContext = context.Resolve<TViewModel>();
                }
            });
        });
        return registrationBuilder;
    }
}
