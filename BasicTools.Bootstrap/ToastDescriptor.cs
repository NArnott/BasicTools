using System;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Bootstrap
{
    public class ToastDescriptor
    {
        public RenderFragment Header { get; init; }

        public RenderFragment Body { get; init; }

        public ToastLevels? Level { get; init; }

        public TimeSpan? AutoHideDelay { get; init; }

        public bool? HideOnNavigation { get; init; }

        public object Icon { get; init; }

        public void HideToast()
        {
            OnHideRequested?.Invoke();
        }

        public event Action OnHideRequested;
    }
}
