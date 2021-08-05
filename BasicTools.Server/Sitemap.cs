using System.Linq;
using System.Reflection;
using System.Text;
using BasicTools.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BasicTools.Server
{
    public static class Sitemap
    {
        public static void MapSitemap(this IEndpointRouteBuilder endpoints, params Assembly[] sourceAssemblies)
        {
            var sp = endpoints.ServiceProvider;

            var memCache = sp.GetRequiredService<IMemoryCache>();
            var routeSources = sp.GetRequiredService<RouteSourceAssemblyProvider>();

            endpoints.MapGet("/sitemap", async context =>
            {
                var sitemap = memCache.GetOrCreate("sitemap", _ => GenerateSitemap(sourceAssemblies, routeSources));

                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(sitemap);
            });
        }

        static string GenerateSitemap(Assembly[] sourceAssemblies, RouteSourceAssemblyProvider routeSources)
        {
            var sb = new StringBuilder();

            foreach (var (_, routeAttribute) in routeSources.PageRoutes.OrderBy(x => x.RouteAttribute.Template))
            {
                sb.AppendLine($"https://basictools.dev{routeAttribute.Template}");
            }

            return sb.ToString();
        }
    }
}
