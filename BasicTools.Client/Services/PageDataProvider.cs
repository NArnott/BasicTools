using System;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Client.Services
{
    class PageDataProvider
    {
        const string SITE_TITLE = "Basic Dev Tools";

        private readonly RouteSourceAssemblyProvider _routeSourceAssemblyProvider;

        public PageDataProvider(
            RouteSourceAssemblyProvider routeSourceAssemblyProvider,
            NavigationManager navigationManager
            )
        {
            _routeSourceAssemblyProvider = routeSourceAssemblyProvider;

            navigationManager.LocationChanged += (sender, args) => OnLocationChanged(navigationManager.ToBaseRelativePath(args.Location));

            OnLocationChanged(navigationManager.ToBaseRelativePath(navigationManager.Uri));
        }

        private void OnLocationChanged(string location)
        {
            if (_routeSourceAssemblyProvider.PageMetadata.TryGetValue("/" + location, out var metadataAttrib))
            {
                if (String.IsNullOrEmpty(metadataAttrib.Title))
                    Title = SITE_TITLE;
                else
                    Title = metadataAttrib.Title;

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

        public event Action? PageChanged;

        public string? Title { get; private set; }

        public string? MetaDescription { get; private set; }

        public string? MetaKeywords { get; private set; }
    }
}
