using System.Text.Json.Serialization;

namespace HARbinger.Models
{
    public  class GraphResponse
    {
        public GraphRequest Request { get; set; }

        [JsonPropertyName("query")]
        public string Content { get; set; }
    }
}
