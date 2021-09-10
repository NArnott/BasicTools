using System.Runtime.CompilerServices;
using BasicTools.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("BasicTools.Tests")]

namespace BasicTools.Client
{
    public class Program
    {
        private static bool _isPreRender = true;

        public static async Task Main(string[] args)
        {
            _isPreRender = false;

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var services = builder.Services;

            ConfigureServices(services);
            
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHeadElementHelper();

            services.AddSingleton(new HostType() { IsPreRender = _isPreRender });

            services.AddSingleton(new RouteSourceAssemblyProvider(typeof(App).Assembly));

            services.AddMudServices(x =>
            {
                x.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

                x.SnackbarConfiguration.PreventDuplicates = true;
                x.SnackbarConfiguration.ShowTransitionDuration = 250;
                x.SnackbarConfiguration.HideTransitionDuration = 500;
            });
        }
    }
}
