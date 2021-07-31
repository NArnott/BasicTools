using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SimpleDevToolbox.Client.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
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
                _guids[i] = new GuidModel(_createGuidModel.Template, i + 1, false);
            }

            _editContext.MarkAsUnmodified();
        }

        public async Task SelectGuid(GuidModel model)
        {
            foreach (var guid in _guids) guid.IsActive = false;

            model.IsActive = true;

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", model.ToString());

            await NotificationService.Info(model.ToString(), "Copied GUID to Clipboard");
        }
    }

    public class GuidModel
    {
        private static readonly RandomNumberGenerator _numberGenerator = RandomNumberGenerator.Create();

        private readonly string _template;
        private readonly int _index;
        private readonly Guid _guid;

        public GuidModel(string template, int index, bool emptyGuid)
        {
            _template = template;
            _index = index;

            _guid = emptyGuid ? Guid.Empty : CreateGuid();
        }

        static Guid CreateGuid()
        {
            var buffer = new byte[16];
            _numberGenerator.GetBytes(buffer);

            return new Guid(buffer);
        }


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

        public override string ToString()
        {
            try
            {
                return FormatString(_template, _guid, _index);
            }
            catch
            {
                return "Invalid Template";
            }
        }

        public static string FormatString(string template, Guid guid, int index)
        {
            return String.Format(template, guid, index);
        }
    }

    public class CreateGuidModel
    {
        public CreateGuidModel()
        {
            Template = "{0}";
        }

        [Required]
        [Range(1, 250)]
        public int CreateGuidCount { get; set; } = 1;

        string _template;

        [Required]
        //[MaxLength(25)]
        //[CustomValidation(typeof(GuidModel), nameof(GuidModel.ValidateCategoryAndPriority))]
        [ValidateTemplate]
        public string Template
        {
            get => _template;
            set 
            {
                _template = value;

                Sample = new GuidModel(Template, 1, true);
            }
        }

        public GuidModel Sample { get; private set; }
    }

    public class ValidateTemplateAttribute : ValidationAttribute
    {
        public ValidateTemplateAttribute() : base("Invalid Template") { }

        public override bool IsValid(object value)
        {
            try
            {
                GuidModel.FormatString((string)value, Guid.Empty, 0);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
