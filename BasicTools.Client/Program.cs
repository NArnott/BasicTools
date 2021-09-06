using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BasicTools.Client.Shared;
using BasicTools.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

[assembly: InternalsVisibleTo("BasicTools.Tests")]

namespace BasicTools.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<HeadSection>("head::after");
            builder.RootComponents.Add<App>("#app");
            
            var services = builder.Services;
            
            services.AddSharedServices(false, typeof(App).Assembly);

            await builder.Build().RunAsync();
        }
    }
}
