using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
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
            var memCache = endpoints.ServiceProvider.GetRequiredService<IMemoryCache>();

            endpoints.MapGet("/sitemap", async context =>
            {
                var sitemap = memCache.GetOrCreate("sitemap", _ => GenerateSitemap(sourceAssemblies));

                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(sitemap);
            });
        }

        static string GenerateSitemap(Assembly[] sourceAssemblies)
        {
            var routeAttributes = from assembly in sourceAssemblies
                                  from type in assembly.ExportedTypes
                                  from attrib in (RouteAttribute[])Attribute.GetCustomAttributes(type, typeof(RouteAttribute))
                                  where type.IsSubclassOf(typeof(ComponentBase))
                                  select attrib;

            var sb = new StringBuilder();

            foreach (var route in routeAttributes)
            {
                sb.AppendLine($"https://basictools.dev{route.Template}");
            }

            return sb.ToString();
        }
    }
}
