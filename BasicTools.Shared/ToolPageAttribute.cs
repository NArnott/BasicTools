using System;
using System.Collections.Concurrent;
using System.Reflection;
using MudBlazor;

namespace BasicTools.Shared
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolPageAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<string, Func<string>> _mudIconCache = new();

        public ToolPageAttribute(string name, string description, string icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }

        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }

        public string GetMudIcon()
        {
            if (Icon == null)
                return null;

            static Func<string> MudIconProviderFactory(string icon)
            {                
                var iconParts = icon.Split('.');

                object curObj = typeof(Icons);

                foreach (var part in iconParts)
                {
                    if (part == "Icons")
                        continue;

                    object newObj = curObj switch
                    {
                        Type type => type.GetMember(part)[0],
                        _ => curObj.GetType().GetMember(part)[0]
                    };

                    if (newObj is Type)
                        curObj = newObj;
                    else if (newObj is FieldInfo fieldInfo)
                        curObj = fieldInfo.GetValue(null);
                    else if (newObj is PropertyInfo propInfo)
                        return () => (string)propInfo.GetValue(curObj);
                }

                throw new InvalidOperationException("Icon not found.");                
            }

            var mudIconProvider = _mudIconCache.GetOrAdd(Icon, MudIconProviderFactory);

            return mudIconProvider();
        }
    }
}
