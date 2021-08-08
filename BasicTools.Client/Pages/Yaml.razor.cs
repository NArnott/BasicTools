using System;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using BasicTools.Bootstrap.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using YamlDotNet.Core;

namespace BasicTools.Client.Pages
{
    partial class Yaml
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        static readonly YamlDotNet.Serialization.Deserializer _yamlDeserializer = new();
        static readonly YamlDotNet.Serialization.Serializer _yamlSerializer = new();

        public string Input { get; set; } = "test: value";

        public string Output { get; set; }

        public bool OutputIsError { get; private set; }

        public bool WrapOutput { get; private set; }

        YamlOutputModes OutputMode { get; set; } = YamlOutputModes.Yaml;

        public void Process()
        {
            ExpandoObject document = null;

            Output = null;
            OutputIsError = false;

            try
            {
                document = _yamlDeserializer.Deserialize<ExpandoObject>(Input);
            }
            catch (YamlException ex)
            {
                Output = ex.Message;
                OutputIsError = true;
            }

            if (document != null)
            {
                Output = OutputMode switch
                {
                    YamlOutputModes.Json => JsonConvert.SerializeObject(document, Formatting.Indented),
                    YamlOutputModes.JsonMinified => JsonConvert.SerializeObject(document),
                    YamlOutputModes.Yaml => _yamlSerializer.Serialize(document),
                    YamlOutputModes.Xml => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.Indented),
                    YamlOutputModes.XmlMinified => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.None),
                    _ => throw new NotImplementedException(),
                };
            }

            WrapOutput = OutputIsError || OutputMode == YamlOutputModes.JsonMinified || OutputMode == YamlOutputModes.XmlMinified;
        }

        async Task CopyOutput()
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Output);

            ToastService.ShowToast("Copied Output to Clipboard", "content_copy");
        }
    }

    public enum YamlOutputModes
    {
        Yaml,

        Json,

        [Description("Json (Minified)")]
        JsonMinified,

        [Description("XML")]
        Xml,

        [Description("XML (Minified)")]
        XmlMinified
    }
}
