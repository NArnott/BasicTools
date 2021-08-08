using System;

namespace BasicTools.Bootstrap.Services
{
    class ToastService : IToastService
    {
        public event Action<ToastDescriptor> OnShow;

        IToastHandle IToastService.ShowToast(ToastDescriptor toast)
        {
            if (toast is null)
                throw new ArgumentNullException(nameof(toast));
            if (toast.Header is null)
                throw new ArgumentException("ToastDescription.Header must be set.", nameof(toast));

            OnShow?.Invoke(toast);

            return new ToastHandle(toast.HideToast);
        }

        class ToastHandle : IToastHandle
        {           
            private Action _onHideCallback;

            public ToastHandle(Action onHideCallback)
            {
                _onHideCallback = onHideCallback;
            }

            public void Dispose()
            {
                if (_onHideCallback != null)
                {
                    _onHideCallback();
                    _onHideCallback = null;
                }
            }
        }
    }
}
