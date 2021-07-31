using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SimpleDevToolbox.Client.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SimpleDevToolbox.Client.Pages
{
    partial class Guids
    {
        private readonly CreateGuidModel _createGuidModel = new();
        private readonly EditContext _editContext;
        
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        INotificationService NotificationService { get; set; }
        
        private GuidModel[] _guids;

        public Guids()
        {
            _editContext = new EditContext(_createGuidModel);
            _editContext.SetFieldCssClassProvider(new BootstrapCssClassProvider());
        }

        private void CreateGuids()
        {
            _guids = new GuidModel[_createGuidModel.CreateGuidCount];

            for (int i = 0; i < _createGuidModel.CreateGuidCount; i++)
            {
                _guids[i] = new GuidModel();
            }

            _editContext.MarkAsUnmodified();
        }

        public async Task SelectGuid(GuidModel model)
        {
            foreach (var guid in _guids) guid.IsActive = false;

            model.IsActive = true;

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", model.Value.ToString());

            await NotificationService.Info(model.Value.ToString(), "Copied GUID to Clipboard");
        }
    }

    public class GuidModel
    {
        public Guid Value { get; } = Guid.NewGuid();

        bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;

                if (value)
                    _wasActive = true;
            }
        }

        bool _wasActive;

        public bool WasActive => _wasActive && !_isActive;
    }

    public class CreateGuidModel
    {
        [Required]
        [Range(1, 250)]
        public int CreateGuidCount { get; set; } = 1;
    }
}
