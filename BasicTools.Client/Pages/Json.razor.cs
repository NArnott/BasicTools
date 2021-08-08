using System;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using BasicTools.Bootstrap.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace BasicTools.Client.Pages
{
    partial class Json
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IToastService ToastService { get; set; }

        static readonly YamlDotNet.Serialization.Serializer _yaml = new();

        public string Input { get; set; } = @"{""test"":""value""}";

        public string Output { get; set; }

        public bool OutputIsError { get; private set; }

        public bool WrapOutput { get; private set; }

        JsonOutputModes OutputMode { get; set; } = JsonOutputModes.Json;

        public void Process()
        {
            ExpandoObject document = null;

            Output = null;
            OutputIsError = false;

            try
            {
                document = JsonConvert.DeserializeObject<ExpandoObject>(Input);
            }
            catch (JsonSerializationException ex)
            {
                Output = ex.Message.Replace("ExpandoObject", "document");
                OutputIsError = true;
            }
            catch (JsonException ex)
            {
                Output = ex.Message;
                OutputIsError = true;
            }
            catch (InvalidCastException)
            {
                Output = "Invalid JSON document";
                OutputIsError = true;
            }

            if (document != null)
            {
                Output = OutputMode switch
                {
                    JsonOutputModes.Json => JsonConvert.SerializeObject(document, Formatting.Indented),
                    JsonOutputModes.JsonMinified => JsonConvert.SerializeObject(document),
                    JsonOutputModes.Yaml => _yaml.Serialize(document),
                    JsonOutputModes.Xml => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.Indented),
                    JsonOutputModes.XmlMinified => XmlExpandoSerializer.Serialize(document, System.Xml.Formatting.None),
                    _ => throw new NotImplementedException(),
                };
            }

            WrapOutput = OutputIsError || OutputMode == JsonOutputModes.JsonMinified || OutputMode == JsonOutputModes.XmlMinified;
        }

        async Task CopyOutput()
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Output);

            ToastService.ShowToast("Copied Output to Clipboard", "content_copy");
        }
    }

    public enum JsonOutputModes
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
