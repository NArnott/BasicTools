using System;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Bootstrap.Services
{
    public static class ToastServiceExtensions
    {
        public static IToastHandle ShowToast(this IToastService toastService, string header, object icon)
        {
            return toastService.ShowToast(new ToastDescriptor()
            {
                Header = header.ToRenderFragment(),
                Icon = icon
            });
        }

        public static IToastHandle ShowToast(this IToastService toastService, string header, TimeSpan autoHideDelay, object icon)
        {
            return toastService.ShowToast(new ToastDescriptor()
            {
                Header = header.ToRenderFragment(),
                AutoHideDelay = autoHideDelay,
                Icon = icon
            });
        }

        public static IToastHandle ShowToast(this IToastService toastService, string header, string body, object icon)
        {
            return toastService.ShowToast(new ToastDescriptor()
            {
                Header = header.ToRenderFragment(),
                Body = body.ToRenderFragment(),
                Icon = icon
            });
        }

        public static IToastHandle ShowToast(this IToastService toastService, string header, string body, TimeSpan autoHideDelay, object icon)
        {
            return toastService.ShowToast(new ToastDescriptor()
            {
                Header = header.ToRenderFragment(),
                Body = body.ToRenderFragment(),
                AutoHideDelay = autoHideDelay,
                Icon = icon
            });
        }

        public static IToastHandle ShowToast(this IToastService toastService, ToastLevels level, string header, string body, TimeSpan autoHideDelay, bool hideOnNavigation, object icon)
        {
            return toastService.ShowToast(new ToastDescriptor()
            {
                Level = level,
                Header = header.ToRenderFragment(),
                Body = body.ToRenderFragment(),
                AutoHideDelay = autoHideDelay,
                HideOnNavigation = hideOnNavigation,
                Icon = icon
            });
        }

        static RenderFragment ToRenderFragment(this string textContent)
        {
            if (textContent == null)
                return null;

            return rtb =>
            {
                rtb.AddContent(0, textContent);
            };
        }
    }
}
