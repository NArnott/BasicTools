using System;

namespace BasicTools.Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolPageAttribute : Attribute
    {
        public ToolPageAttribute(string name, string description, object faIcon)
        {
            Name = name;
            Description = description;
            FaIcon = faIcon;
        }

        public string Name { get; }
        public string Description { get; }
        public object FaIcon { get; }
    }
}
