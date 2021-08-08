using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Bootstrap.Components
{
    partial class Toast
    {
        [Parameter]
        public ToastDescriptor ToastDescriptor { get; set; }


        [Inject]
        NavigationManager NavManager { get; set; }

        [CascadingParameter]
        ToastContainer Container { get; set; }


        const string CLASS_NAME_SHOWING = "showing";

        private string ToastClass => _showing || _hiding ? CLASS_NAME_SHOWING : null;

        private string ToastLevel
        {
            get
            {
                var level = ToastDescriptor.Level ?? Container.Defaults.Level;

                return level == ToastLevels.Default ? null : $"bg-{level.ToString().ToLower()}";
            }
        }
        
        bool _showing = true;
        bool _hiding = false;

        protected override void OnInitialized()
        {
            if (ToastDescriptor.Icon != null && Container.IconTemplate == null)
                throw new InvalidOperationException("Toast has provided an Icon, but no ToastContainer.IconTemplate has been specified.");

            ToastDescriptor.OnHideRequested += HideToast;

            if (ToastDescriptor.HideOnNavigation ?? Container.Defaults.HideOnNavigation)
                NavManager.LocationChanged += (_, _) => HideToast();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_showing)
            {
                await Task.Delay(10);

                _showing = false;
                StateHasChanged();

                SetupAutoHide();

                return;
            }

            if (_hiding)
            {
                await Task.Delay(500);

                Container.RemoveToast(ToastDescriptor);
            }
        }

        void SetupAutoHide()
        {
            var autoHideDelay = ToastDescriptor.AutoHideDelay ?? Container.Defaults.AutoHideDelay;

            if (autoHideDelay.HasValue)
            {
                var timer = new Timer(autoHideDelay.Value.TotalMilliseconds)
                {
                    AutoReset = false
                };

                timer.Elapsed += (_, __) =>
                {
                    timer.Dispose();

                    HideToast();
                };

                timer.Start();
            }
        }

        private void HideToast()
        {
            _hiding = true;
            StateHasChanged();
        }
    }
}
