using System.Text.Json;

namespace HARbinger.Models
{
    public class GraphEntryModel
    {
        private GraphRequest _graphRequest;

        public string OperationName { get; }
        public string OperationType { get; }

        public List<string> Responses { get; }

        public GraphEntryModel(IGrouping<string, Entries> entries)
        {
            OperationName = entries.Key;
            _graphRequest = JsonSerializer.Deserialize<GraphRequest>(entries.First().request.postData.text);
            OperationType = _graphRequest.Query.Split(' ')[0];

            Responses = new List<string>();
            foreach(var entry in entries)
            {
                Responses.Add(entry.response.content.text);
            }
        }
    }
}
