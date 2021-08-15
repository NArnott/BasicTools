using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BasicTools.Server
{
    static class EndpointExtensions
    {
        public static void MapRedirect(this IEndpointRouteBuilder endpoints, string source, string redirectTo)
        {
            endpoints.MapGet(source, context =>
            {
                context.Response.Redirect(redirectTo, true);

                return Task.CompletedTask;
            });
        }
    }
}
