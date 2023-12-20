using HARbinger.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;

namespace HARbinger.Services
{
    public static class HarFileService
    {
        private static long MaxFileSize = 1024L * 1024L * 1024L; // 1GB
        private static string TempFilePath => AppDomain.CurrentDomain.BaseDirectory + "har.json";

        public static RootObject HarData { get; set; }

        public static IEnumerable<GraphEntryModel> GetAllGraphEntries()
        {
            // Get all entries that make calls to /graph or /graphauxiliary
            var graphEntries = HarData.log.entries
                    .Where(entry =>
                           (entry.request?.url?.EndsWith("graph") == true || entry.request?.url?.EndsWith("graphauxiliary") == true) &&
                           entry.request?.postData?.text != null);

            // Group entries by operation name
            var groupedGraphEntries = graphEntries
                    .GroupBy(graphEntry => JsonSerializer.Deserialize<GraphRequest>(graphEntry.request.postData.text).OperationName.ToLower());

            var entryModels = groupedGraphEntries.Select(group =>
                           new GraphEntryModel(group));

            return entryModels;
        }

        public static IEnumerable<IGrouping<string, Entries>> GetGroupedGraphEntries()
        {
            // Get all entries that make calls to /graph or /graphauxiliary
            var graphEntries = HarData.log.entries
                    .Where(entry =>
                           (entry.request?.url?.EndsWith("graph") == true || entry.request?.url?.EndsWith("graphauxiliary") == true) &&
                           entry.request?.postData?.text != null);

            // Group entries by operation name
            var groupedGraphEntries = graphEntries
                    .GroupBy(graphEntry => JsonSerializer.Deserialize<GraphRequest>(graphEntry.request.postData.text).OperationName.ToLower());

            return groupedGraphEntries;
        }

        public static List<string> GetGraphRequests()
        {
            var graphRequests = 
                HarData.log.entries
                    .Where(entry => entry.request?.url?.Contains("graph") == true && 
                           entry.request?.postData?.text != null)
                    .Select(entry => JsonSerializer.Deserialize<GraphRequest>(entry.request.postData.text)).ToArray();
            return graphRequests.GroupBy(graphRequest =>
                graphRequest.OperationName).Select(group =>
                group.First().OperationName).OrderBy(operationName =>
                operationName).ToList();
        }

        internal static Mock GetMockAsync(string mockName, List<string> graphOperationNames)
        {
            var graphResponses = HarData.log.entries.Where(entry =>
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

        public static async Task<List<string>> Process(IBrowserFile browserFile)
        {
            var errors = new List<string>();

            if (browserFile == null)
            {
                errors.Add("No file selected.");
                return errors;
            }

            if (browserFile.Size > MaxFileSize)
            {
                errors.Add("File is too large.");
                return errors;
            }

            try
            {
                await using FileStream fileStream = new(TempFilePath, FileMode.Create);
                await browserFile.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);

                fileStream.Seek(0, SeekOrigin.Begin);

                using (StreamReader sr = new(fileStream))
                {
                    HarData = JsonSerializer.Deserialize<RootObject>(sr.ReadToEnd());
                }
                return errors;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return errors;
            }
        }

        private static MockResponseType GetMockResponseTypeByName(string name) => name.ToLowerInvariant() switch
        {
            "mutation" => MockResponseType.Mutation,
            _ => MockResponseType.Query,
        };
    }
}
