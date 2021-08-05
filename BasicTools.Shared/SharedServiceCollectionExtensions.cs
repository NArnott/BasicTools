using System;
using System.Reflection;
using BasicTools.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BasicTools.Shared
{
    public static class SharedServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, params Assembly[] sourceAssemblies)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton(new RouteSourceAssemblyProvider(sourceAssemblies));
            services.AddScoped<PageDataProvider>();

            return services;
        }
    }
}
