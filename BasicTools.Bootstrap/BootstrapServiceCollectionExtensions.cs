using BasicTools.Bootstrap.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BootstrapServiceCollectionExtensions
    {
        public static IServiceCollection AddToastService(this IServiceCollection services)
        {
            services.AddScoped<IToastService, ToastService>();

            return services;
        }
    }
}
