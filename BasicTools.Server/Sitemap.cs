using System.Linq;
using System.Reflection;
using System.Text;
using BasicTools.Shared;
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
        public static void MapSitemap(this IEndpointRouteBuilder endpoints)
        {
            var sp = endpoints.ServiceProvider;

            var memCache = sp.GetRequiredService<IMemoryCache>();
            var routeSources = sp.GetRequiredService<RouteSourceAssemblyProvider>();

            endpoints.MapGet("/sitemap", async context =>
            {
                var sitemap = memCache.GetOrCreate("sitemap", _ => GenerateSitemap(routeSources));

                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(sitemap);
            });
        }

        static string GenerateSitemap(RouteSourceAssemblyProvider routeSources)
        {
            var sb = new StringBuilder();

            foreach (var (pageType, routeAttribute) in routeSources.PageRoutes.OrderBy(x => x.RouteAttribute.Template))
            {
                if (pageType.GetCustomAttribute(typeof(RemoveFromSitemapAttribute)) != null)
                    continue;

                sb.AppendLine($"https://basictools.dev{routeAttribute.Template}");
            }

            return sb.ToString();
        }
    }
}
