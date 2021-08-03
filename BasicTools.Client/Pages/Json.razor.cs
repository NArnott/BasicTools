using System;
using System.Text.Json;

namespace BasicTools.Client.Pages
{
    partial class Json
    {
        string Input { get; set; } = @"{""test"":""value""}";

        string Output { get; set; }

        bool OutputIsError { get; set; }

        public void Process()
        {
            dynamic result;

            try
            {
                result = JsonSerializer.Deserialize<dynamic>(Input);
            }
            catch (Exception ex)
            {
                Output = ex.Message;
                OutputIsError = true;
                return;
            }

            Output = JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions()
                {
                     WriteIndented = true
                }
            );
            OutputIsError = false;
        }
    }
}
