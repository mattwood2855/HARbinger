using System.Text.Json.Serialization;

namespace HARbinger.Models
{
    public class GraphRequest
    {
        [JsonPropertyName("operationName")]
        public string OperationName { get; set; }

        [JsonPropertyName("query")]
        public string Query { get; set; }
    }
}
