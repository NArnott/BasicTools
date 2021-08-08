using System;

namespace BasicTools.Bootstrap
{
    public class ToastDefaults
    {
        public ToastLevels Level { get; init; } = ToastLevels.Default;

        public TimeSpan? AutoHideDelay { get; init; }

        public bool HideOnNavigation { get; init; } = false;
    }
}
