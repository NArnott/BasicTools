using System;

namespace BasicTools.Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolPageAttribute : Attribute
    {
        public ToolPageAttribute(string name, string description, string icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }

        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }
    }
}
