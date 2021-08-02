using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Shared.Services
{
    public class HeadStateService
    {
        const string SITE_TITLE = "Basic Dev Tools";

        static Dictionary<string, PageMetadata> Metadata { get; }

        static HeadStateService()
        {
            Metadata = GetPageMetadata();
        }

        private static Dictionary<string, PageMetadata> GetPageMetadata()
        {
            return new Dictionary<string, PageMetadata>(StringComparer.InvariantCultureIgnoreCase)
            {
                [""] = new PageMetadata()
                {
                    Title = null,
                    MetaDescription = "A basic set of free, online tools to help out developers",
                },
                ["guids"] = new PageMetadata()
                {
                    Title = "GUIDs",
                    MetaDescription = "Create GUIDs",
                    MetaKeywords = "guid, uuid, guid generator, uuid generator, online guid, onlineguid, globally unique identifier, universal unique identifier, java guid, java uuid, C# guid, C# uuid, globally unique identifier, unique identifier"
                }
            };
        }

        public HeadStateService(NavigationManager navigationManager)
        {
            navigationManager.LocationChanged += (sender, args) => OnLocationChanged(navigationManager.ToBaseRelativePath(args.Location));

            OnLocationChanged(navigationManager.ToBaseRelativePath(navigationManager.Uri));
        }

        private void OnLocationChanged(string location)
        {
            if (Metadata.TryGetValue(location, out var metadata))
            {
                if(String.IsNullOrEmpty(metadata.Title))
                    Title = SITE_TITLE;
                else
                    Title = $"{metadata.Title} - {SITE_TITLE}";

                MetaDescription = metadata.MetaDescription;
                MetaKeywords = metadata.MetaKeywords;
            }
            else
            {
                Title = SITE_TITLE;
                MetaDescription = null;
                MetaKeywords = null;
            }

            StateChanged?.Invoke();
        }

        public event Action StateChanged;

        public string Title { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        class PageMetadata
        {
            public string Title;

            public string MetaDescription;

            public string MetaKeywords;
        }
    }
}
