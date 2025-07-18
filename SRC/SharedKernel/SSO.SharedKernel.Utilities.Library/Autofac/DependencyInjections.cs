﻿using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using SSO.SharedKernel.Utilities.Library.Autofac.Abstractions;
using System.Reflection;

namespace SSO.SharedKernel.Utilities.Library.Autofac;

public static class DependencyInjection
{
    public static ContainerBuilder AddAutofacLifetimeServices(this ContainerBuilder builder, Assembly[] assemblies)
    {
        // Get the assembly where your services are located
        // Register services based on their lifetime interfaces
        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => t.IsAssignableTo<IAutofacSingletonLifetime>())
            .AsImplementedInterfaces()
            .SingleInstance();

        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => t.IsAssignableTo<IAutofacScopedLifetime>())
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => t.IsAssignableTo<IAutofacTransientLifetime>())
            .AsImplementedInterfaces()
            .InstancePerDependency();

        return builder;
    }

    public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services, Assembly[] assemblies)
    {
        var builder = new ContainerBuilder();

        // Populate Autofac with existing ASP.NET Core DI registrations
        builder.Populate(services);

        // Add our lifetime-based registrations
        builder.AddAutofacLifetimeServices(assemblies);

        var container = builder.Build();
        return new AutofacServiceProvider(container);
    }
}
