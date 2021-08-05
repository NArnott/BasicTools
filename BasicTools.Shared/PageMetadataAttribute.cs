using System;

namespace BasicTools.Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PageMetadataAttribute : Attribute
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }
    }
}
