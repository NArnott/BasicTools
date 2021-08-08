using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BasicTools.Bootstrap.Services;
using BasicTools.Client.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BasicTools.Client.Pages
{
    partial class Guids
    {
        private readonly CreateGuidModel _createGuidModel = new();
        private readonly EditContext _editContext;

        private GuidModel[] _guids;

        private readonly ExampleTemplate[] _exampleTemplates;

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        public Guids()
        {
            _editContext = new EditContext(_createGuidModel);
            _editContext.SetFieldCssClassProvider(new BootstrapCssClassProvider());
            
            ExampleTemplate CreateTemplate(string name, string template, string hint = null)
            {
                return new ExampleTemplate(name, template, hint, t =>
                {
                    _createGuidModel.Template = t;

                    _editContext.NotifyValidationStateChanged();
                });
            }
            
            _exampleTemplates = new[]
            {
                CreateTemplate("Standard GUID with hyphens", "{0}"),
                CreateTemplate("Uppercase GUID", "{0:U}", "Add a U formatter to any guid value to make it upper case. Ex: {0:BU} or {0:UN}"),
                CreateTemplate("GUID with Braces", "{0:B}"),
                CreateTemplate("GUID without hyphens", "{0:N}"),

                CreateTemplate("C# GUID Variable", "var guid{1} = Guid.Parse(\"{0}\");"),
            };
        }

        private void CreateGuids()
        {
            _guids = new GuidModel[_createGuidModel.CreateGuidCount];

            for (int i = 0; i < _createGuidModel.CreateGuidCount; i++)
            {
                _guids[i] = new GuidModel(_createGuidModel.Template, i + 1, _createGuidModel.GenerationMode, false);
            }
            
            _editContext.MarkAsUnmodified();
        }

        public async Task SelectGuid(GuidModel model)
        {
            foreach (var guid in _guids) guid.IsActive = false;

            model.IsActive = true;

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", model.ToString());

            ToastService.ShowToast("Copied GUID to Clipboard", model.ToString(), "content_copy");
        }

        IToastHandle _copiedAllGuidsToastHandle;

        public async Task CopyAllGuids()
        {
            var allGuids = string.Join(Environment.NewLine, _guids.Select(x => x.ToString()));

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", allGuids);

            _copiedAllGuidsToastHandle?.Dispose();

            _copiedAllGuidsToastHandle = ToastService.ShowToast("Copied all GUIDs to Clipboard", "content_copy");

            foreach (var guid in _guids) guid.MarkWasActive();
        }

        public class GuidModel
        {
            private static readonly RandomNumberGenerator _numberGenerator = RandomNumberGenerator.Create();

            private static readonly Guid _sampleGuid = Guid.Parse("00000000-aaaa-ffff-9999-000000000000");

            private readonly string _template;
            private readonly int _index;
            private readonly Guid _guid;

            public GuidModel(string template, int index, GenerationModes genMode, bool sampleGuid)
            {
                _template = template;
                _index = index;

                _guid = sampleGuid ? _sampleGuid : CreateGuid(genMode);
            }

            static Guid CreateGuid(GenerationModes genMode)
            {
                switch (genMode)
                {
                    case GenerationModes.Default:
                        return Guid.NewGuid();

                    case GenerationModes.Cryptographic:
                        var buffer = new byte[16];
                        _numberGenerator.GetBytes(buffer);

                        return new Guid(buffer);
                }

                throw new NotImplementedException();
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

            public void MarkWasActive() => _wasActive = true;

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
                return String.Format(CustomGuidFormatter.Instance, template, guid, index);
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
            [MaxLength(250)]
            [ValidateTemplate]
            public string Template
            {
                get => _template;
                set
                {
                    _template = value;

                    Sample = new GuidModel(value, 1, GenerationModes.Default, true);
                }
            }


            public GenerationModes GenerationMode { get; set; } = GenerationModes.Default;

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

        public class ExampleTemplate
        {
            public ExampleTemplate(string name, string template, string hint, Action<string> onSelect)
            {
                Name = name;
                Template = template;
                Hint = hint;

                Sample = new GuidModel(template, 1, GenerationModes.Default, true);

                _onSelect = onSelect;
            }

            public string Name { get; }

            public string Template { get; }

            public string Hint { get; }

            private readonly Action<string> _onSelect;

            public GuidModel Sample { get; private set; }

            public void Select()
            {
                _onSelect(Template);
            }
        }

        public class CustomGuidFormatter : IFormatProvider, ICustomFormatter
        {
            private CustomGuidFormatter() { }

            public static readonly CustomGuidFormatter Instance = new();

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                else
                    return null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg is not Guid guid)
                {
                    if (arg is IFormattable formattable)
                        return formattable.ToString(format == null ? null : "{0:" + format + "}", null);
                    else
                        throw new FormatException();
                }

                bool toUpper = format?.Contains("U", StringComparison.InvariantCultureIgnoreCase) == true;

                if (toUpper)
                    format = format.Replace("U", "");

                var result = guid.ToString(format);

                if (toUpper)
                    result = result.ToUpper();
                
                return result;
            }
        }

        public enum GenerationModes
        {
            [Description("Uses the default .NET implementation for creating GUIDs")]
            Default,

            [Description("Creates cryptographically random GUIDs")]
            Cryptographic
        }
    }


}
