using System.ComponentModel;
using System.Dynamic;
using BasicTools.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using YamlDotNet.Core;

namespace BasicTools.Client.Pages
{
    partial class JsonYaml
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        ISnackbar ToastService { get; set; }

        [Inject]
        HostType HostType { get; set; }

        static readonly YamlDotNet.Serialization.Deserializer _yamlDeserializer = new();
        static readonly YamlDotNet.Serialization.Serializer _yamlSerializer = new();

        public string Input { get; set; } = @"{""test"":""value""}";

        public JsonYamlOutputModes OutputMode { get; set; } = JsonYamlOutputModes.Json;
        
        public OutputResult Output { get; private set; }

        IJSObjectReference _jsonJs;

        public JsonYaml()
        {
            Output = new OutputResult()
            {
                Mode = OutputMode
            };
        }

        protected override async Task OnInitializedAsync()
        {
            if (!HostType.IsPreRender)
            {
                await JSRuntime.InvokeVoidAsync("import", "/scripts/jquery.json-viewer.js");

                _jsonJs = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/scripts/json.js");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!HostType.IsPreRender
                && Output.Mode == JsonYamlOutputModes.Json
                && !Output.IsError
                && Output.OutputText != null
                && !Output.HasBeenRendered
                )
            {
                await _jsonJs.InvokeVoidAsync("displayJson", "#json-renderer", Output.OutputText);
            }

            Output.HasBeenRendered = true;
        }

        public void Process()
        {
            ExpandoObject document = null;

            Output = new OutputResult()
            {
                Mode = OutputMode
            };

            try
            {
                document = _yamlDeserializer.Deserialize<ExpandoObject>(Input);
            }
            catch (YamlException ex)
            {
                Output.OutputText = ex.Message;
                Output.IsError = true;
            }

            if (document != null)
            {
                Output.OutputText = OutputMode switch
                {
                    JsonYamlOutputModes.Json => JsonConvert.SerializeObject(document, Formatting.Indented),
                    JsonYamlOutputModes.JsonMinified => JsonConvert.SerializeObject(document),
                    JsonYamlOutputModes.Yaml => _yamlSerializer.Serialize(document),
                    JsonYamlOutputModes.Xml => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.Indented),
                    JsonYamlOutputModes.XmlMinified => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.None),
                    _ => throw new NotImplementedException(),
                };
            }

            Output.WrapOutput = Output.IsError || OutputMode == JsonYamlOutputModes.JsonMinified || OutputMode == JsonYamlOutputModes.XmlMinified;
        }

        async Task CopyOutput()
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Output.OutputText);

            ToastService.Add("Copied Output to Clipboard", configure: x => x.Icon = Icons.Filled.ContentCopy);
        }

        public class OutputResult
        {
            public string OutputText { get; set; }

            public bool IsError { get; set; }

            public bool WrapOutput { get; set; }

            public JsonYamlOutputModes Mode { get; set; }

            public bool HasBeenRendered { get; set; }
        }
    }

    public enum JsonYamlOutputModes
    {
        Json,

        [Description("Json (Minified)")]
        JsonMinified,

        Yaml,

        [Description("XML")]
        Xml,

        [Description("XML (Minified)")]
        XmlMinified
    }

}
