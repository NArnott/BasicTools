using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Client.Services
{
    class RouteSourceAssemblyProvider
    {
        public RouteSourceAssemblyProvider(params Assembly[] sourceAssemblies)
        {
            SourceAssemblies = Array.AsReadOnly(sourceAssemblies);

            PageRoutes = (
                from pageType in GetPageTypes()
                from routeAttrib in (RouteAttribute[])Attribute.GetCustomAttributes(pageType, typeof(RouteAttribute))
                select (pageType, routeAttrib)
            ).ToArray();

            ToolPages = (
                from route in PageRoutes
                let toolAttrib = (ToolPageAttribute?)Attribute.GetCustomAttribute(route.PageType, typeof(ToolPageAttribute))
                where toolAttrib != null
                select new { route.RouteAttribute, toolAttrib }
            ).ToDictionary(x => x.RouteAttribute.Template, x => x.toolAttrib);
        }

        private IEnumerable<Type> GetPageTypes()
        {
            var routeAttributes = from assembly in SourceAssemblies
                                  from type in assembly.ExportedTypes
                                  where type.IsSubclassOf(typeof(ComponentBase))
                                  select type;

            return routeAttributes;
        }

        public ReadOnlyCollection<Assembly> SourceAssemblies { get; }

        public (Type PageType, RouteAttribute RouteAttribute)[] PageRoutes { get; }

        public IReadOnlyDictionary<string, ToolPageAttribute> ToolPages { get; }
    }
}
