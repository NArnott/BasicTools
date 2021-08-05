using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Shared.Services
{
    public class RouteSourceAssemblyProvider
    {
        public RouteSourceAssemblyProvider(Assembly[] sourceAssemblies)
        {
            SourceAssemblies = Array.AsReadOnly(sourceAssemblies);

            PageRoutes = (
                from pageType in GetPageTypes()
                from routeAttrib in (RouteAttribute[])Attribute.GetCustomAttributes(pageType, typeof(RouteAttribute))
                select (pageType, routeAttrib)
            ).ToArray();
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
    }
}
