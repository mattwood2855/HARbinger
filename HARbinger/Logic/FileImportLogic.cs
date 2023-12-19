using System.Text.Json;
using HARbinger.Models;

namespace HARbinger.Logic
{
    internal static class FileImportLogic
    {
        internal static async Task<string[]> GetGraphOperationNamesFromHarFileAsync(string harFilePath)
        {
            using var file = File.OpenRead(harFilePath);
            var rootObject = await JsonSerializer.DeserializeAsync<RootObject>(file).ConfigureAwait(false);
            var jsons = rootObject.log.entries.Select(entry => entry.response.content.text).ToArray();
            var graphRequests = new List<GraphRequest>();
            await Task.Run(() =>
            {
                foreach (var json in jsons)
                    graphRequests.Add(JsonSerializer.Deserialize<GraphRequest>(json));
            }).ConfigureAwait(false);
            return graphRequests.GroupBy(graphRequest => graphRequest.OperationName).Select(group => group.First().OperationName).ToArray();
        }
    }
}
