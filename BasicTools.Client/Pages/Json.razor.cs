using System;
using System.ComponentModel;
using System.Dynamic;
using Newtonsoft.Json;

namespace BasicTools.Client.Pages
{
    partial class Json
    {
        static readonly YamlDotNet.Serialization.Serializer _yaml = new();

        string Input { get; set; } = @"{""test"":""value""}";

        string Output { get; set; }

        bool OutputIsError { get; set; }

        bool WrapOutput { get; set; }

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
            catch(Exception ex)
            {
                Output = ex.Message;
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
