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
            var graphRequests = rootObject.log.entries.Where(entry =>
                entry.request?.url?.Contains("graph") == true
                    && entry.request?.postData?.text != null).Select(entry =>
                JsonSerializer.Deserialize<GraphRequest>(entry.request.postData.text)).ToArray();
            return graphRequests.GroupBy(graphRequest =>
                graphRequest.OperationName).Select(group =>
                group.First().OperationName).OrderBy(operationName =>
                operationName).ToArray();
        }

        internal static async Task<Mock> GetMockAsync(string harFilePath, string mockName, string[] graphOperationNames)
        {
            using var file = File.OpenRead(harFilePath);
            var rootObject = await JsonSerializer.DeserializeAsync<RootObject>(file).ConfigureAwait(false);
            var graphResponses = rootObject.log.entries.Where(entry =>
                entry.request?.url?.Contains("graph") == true
                    && entry.request?.postData?.text != null
                    && entry.response?.content?.text != null).Select(entry =>
                new GraphResponse
                {
                    Request = JsonSerializer.Deserialize<GraphRequest>(entry.request.postData.text),
                    Content = entry.request.postData.text
                }).Where(graphResponse =>
                graphOperationNames.Contains(graphResponse.Request.OperationName)).GroupBy(graphResponse =>
                graphResponse.Request.OperationName).ToDictionary(group =>
                group.Key, group =>
                group.ToArray());
            var mockResponses = new List<MockResponse>();
            foreach (var graphOperationName in graphOperationNames)
            {
                var graphResponse = graphResponses[graphOperationName];
                mockResponses.Add(new MockResponse
                {
                    Name = graphOperationName,
                    Type = GetMockResponseTypeByName(graphResponse.First().Request.Query.Split()[0]),
                    Contents = graphResponse.Select(graphResponse =>
                        graphResponse.Content).ToArray()
                });
            }
            return new Mock
            {
                Name = mockName,
                Responses = mockResponses.ToArray()
            };
        }

        private static MockResponseType GetMockResponseTypeByName(string name) => name.ToLowerInvariant() switch
        {
            "mutation" => MockResponseType.Mutation,
            _ => MockResponseType.Query,
        };
    }
}
