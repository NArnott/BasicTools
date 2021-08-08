using System;

namespace BasicTools.Bootstrap.Services
{
    public interface IToastService
    {
        event Action<ToastDescriptor> OnShow;

        IToastHandle ShowToast(ToastDescriptor toast);
    }

    public interface IToastHandle : IDisposable
    { 
    }
}
