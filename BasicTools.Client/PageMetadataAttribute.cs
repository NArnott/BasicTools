using System;

namespace BasicTools.Client
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class PageMetadataAttribute : Attribute
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }
    }
}
