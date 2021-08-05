using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Shared.Services
{
    public class PageDataProvider
    {
        const string SITE_TITLE = "Basic Dev Tools";

        private readonly Dictionary<string, PageMetadataAttribute> _pageMetadata;        

        public PageDataProvider(
            RouteSourceAssemblyProvider routeSourceAssemblyProvider,
            NavigationManager navigationManager
            )
        {
            _pageMetadata = (from route in routeSourceAssemblyProvider.PageRoutes
                            let pageMetaAttrib = (PageMetadataAttribute)Attribute.GetCustomAttribute(route.PageType, typeof(PageMetadataAttribute))
                            where pageMetaAttrib != null
                            select new { route.RouteAttribute, pageMetaAttrib }
                            ).ToDictionary(x => x.RouteAttribute.Template, x => x.pageMetaAttrib);

            navigationManager.LocationChanged += (sender, args) => OnLocationChanged(navigationManager.ToBaseRelativePath(args.Location));

            OnLocationChanged(navigationManager.ToBaseRelativePath(navigationManager.Uri));
        }


        private void OnLocationChanged(string location)
        {
            if (_pageMetadata.TryGetValue("/" + location, out var metadataAttrib))
            {
                if (String.IsNullOrEmpty(metadataAttrib.Title))
                    Title = SITE_TITLE;
                else
                    Title = $"{metadataAttrib.Title} - {SITE_TITLE}";

                MetaDescription = metadataAttrib.Description;
                MetaKeywords = metadataAttrib.Keywords;
            }
            else
            {
                Title = SITE_TITLE;
                MetaDescription = null;
                MetaKeywords = null;
            }

            PageChanged?.Invoke();
        }

        public event Action PageChanged;

        public string Title { get; private set; }

        public string MetaDescription { get; private set; }

        public string MetaKeywords { get; private set; }
    }
}
