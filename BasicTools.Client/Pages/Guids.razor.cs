using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BasicTools.Client.Pages
{
    partial class Guids
    {
        private GuidModel[]? _guids;

        private readonly ExampleTemplate[] _exampleTemplates;

        [Inject]
        IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        ISnackbar ToastService { get; set; } = default!;

        public int CreateGuidCount { get; set; } = 1;

        string _template = default!;

        public string Template
        {
            get => _template;
            set
            {
                _template = value;

                Sample = new GuidModel(value, 1, GenerationModes.Default, true);
            }
        }

        public bool IsTemplateValid(string value)
        {
            return Sample.IsValid;
        }

        public GenerationModes GenerationMode { get; set; }

        public GuidModel Sample { get; private set; } = default!;

        public Guids()
        {
            Template = "{0}";

            ExampleTemplate CreateTemplate(string name, string template, string? hint = null)
            {
                return new ExampleTemplate(name, template, hint, t =>
                {
                    Template = t;

                    _form.Validate();
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

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //for some reason, Mud Radio buttons don't validate the default value, so we have to set it after the fact
                GenerationMode = GenerationModes.Default;
                _form.Validate();
            }
        }

        private void CreateGuids()
        {
            _guids = new GuidModel[CreateGuidCount];

            for (int i = 0; i < CreateGuidCount; i++)
            {
                _guids[i] = new GuidModel(Template, i + 1, GenerationMode, false);
            }
        }

        public async Task SelectGuid(GuidModel model)
        {
            if (_guids == null)
                throw new InvalidOperationException("_guids null");

            foreach (var guid in _guids) guid.IsActive = false;

            model.IsActive = true;

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", model.ToString());

            ToastService.Add($"Copied {model} to Clipboard", configure: x => x.Icon = Icons.Filled.ContentCopy);
        }

        public async Task CopyAllGuids()
        {
            if (_guids == null)
                throw new InvalidOperationException("_guids null");

            var allGuids = string.Join(Environment.NewLine, _guids.Select(x => x.ToString()));

            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", allGuids);

            ToastService.Add("Copied all GUIDs to Clipboard", configure: x => x.Icon = Icons.Filled.ContentCopy);

            foreach (var guid in _guids) guid.MarkWasActive();
        }

        public class GuidModel
        {
            private static readonly RandomNumberGenerator _numberGenerator = RandomNumberGenerator.Create();

            private static readonly Guid _sampleGuid = Guid.Parse("00000000-aaaa-ffff-9999-000000000000");

            private readonly string? _output;

            public GuidModel(string template, int index, GenerationModes genMode, bool sampleGuid)
            {

                var guid = sampleGuid ? _sampleGuid : CreateGuid(genMode);

                try
                {
                    _output = FormatString(template, guid, index);
                }
                catch
                {
                    _output = null;
                }
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

            public bool IsValid => _output != null;

            public override string? ToString()
            {
                return _output;
            }

            static string FormatString(string template, Guid guid, int index)
            {
                return String.Format(CustomGuidFormatter.Instance, template, guid, index);
            }
        }

        public class ExampleTemplate
        {
            public ExampleTemplate(string name, string template, string? hint, Action<string> onSelect)
            {
                Name = name;
                Template = template;
                Hint = hint;

                Sample = new GuidModel(template, 1, GenerationModes.Default, true);

                _onSelect = onSelect;
            }

            public string Name { get; }

            public string Template { get; }

            public string? Hint { get; }

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

            public object? GetFormat(Type? formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                else
                    return null;
            }

            public string Format(string? format, object? arg, IFormatProvider? formatProvider)
            {
                if (arg is not Guid guid)
                {
                    if (arg is IFormattable formattable)
                        return formattable.ToString(format == null ? null : "{0:" + format + "}", null);
                    else
                        throw new FormatException();
                }

                bool toUpper = format?.Contains('U', StringComparison.InvariantCultureIgnoreCase) == true;

                if (toUpper && format != null)
                    format = format.Replace("U", "");

                var result = guid.ToString(format);

                if (toUpper)
                    result = result.ToUpper();
                
                return result;
            }
        }

        public enum GenerationModes
        {
            NoSet,

            [Description("Uses the default .NET implementation for creating GUIDs")]
            Default,

            [Description("Creates cryptographically random GUIDs")]
            Cryptographic
        }
    }


}
