using System;
using System.Reflection;
using BasicTools.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace BasicTools.Shared
{
    public static class SharedServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, bool isServer, params Assembly[] sourceAssemblies)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton(new HostType() { IsServer = isServer });
            services.AddSingleton(new RouteSourceAssemblyProvider(sourceAssemblies));
            services.AddScoped<PageDataProvider>();

            services.AddMudServices(x =>
            {
                x.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

                x.SnackbarConfiguration.PreventDuplicates = true;
                x.SnackbarConfiguration.ShowTransitionDuration = 250;
                x.SnackbarConfiguration.HideTransitionDuration = 500;
            });

            return services;
        }
    }
}
