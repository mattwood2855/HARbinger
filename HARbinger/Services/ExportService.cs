using HARbinger.Models;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HARbinger.Services
{
    public class ExportProgress
    {
        public string CurrentRequest { get; set; }
        public float PercentComplete { get; set; }
    }

    public static class ExportService
    {
        private static readonly string _mocksDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}code{Path.DirectorySeparatorChar}spectrum-web{Path.DirectorySeparatorChar}mock_server{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}mock_files{Path.DirectorySeparatorChar}graph";

        private static readonly Dictionary<MockResponseType, string> _mocksDirectoryByType = new()
        {
            {  MockResponseType.Mutation, "mutation" },
            {  MockResponseType.Query, "query" }
        };

        public static Task ExportAllGraphMocks(string username, IProgress<ExportProgress> progress)
        {
            return Task.Run(() =>
            {
                // Get the grouped graph calls
                var graphRequestEntries = HarFileService.GetAllGraphEntries().ToList();

                // Make sure the mocks graph directory exists
                Directory.CreateDirectory(RepoService.GetMockPath());

                // Go through each operation
                for (var index = 0; index < graphRequestEntries.Count; index++)
                {
                    UpdateProgress(progress, index, graphRequestEntries.Count + 1, graphRequestEntries[index].OperationName);

                    var opType = graphRequestEntries[index].OperationType;
                    var opName = graphRequestEntries[index].OperationName;
                    var newDirectory = $"{RepoService.GetMockPath()}{Path.DirectorySeparatorChar}{opType}{Path.DirectorySeparatorChar}{opName}{Path.DirectorySeparatorChar}post{Path.DirectorySeparatorChar}";
                    Directory.CreateDirectory(newDirectory);
                    var newFile = $"{username}.json";
                    var newFileContent = $"{{\n  \"orchestratedResponses\": [{string.Join(",\n", graphRequestEntries[index].Responses)}]\n}}";
                    var tempObj = JsonSerializer.Deserialize<JsonElement>(newFileContent);
                    var formattedContent = JsonSerializer.Serialize(tempObj, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText($"{newDirectory}{newFile}", formattedContent);
                }

                UpdateProgress(progress, 1, 1, "Complete");
            });
        }

        private static void UpdateProgress(IProgress<ExportProgress> progress, int index, int total, string operationName)
        {
            if (progress != null)
            {
                var report = new ExportProgress
                {
                    CurrentRequest = operationName,
                    PercentComplete = index / total
                };
                progress.Report(report);
            }
        }

        internal static async Task ExportMockAsync(Mock mock, IProgress<ExportProgress> progress, string mocksDirectory = null)
        {
            mocksDirectory ??= _mocksDirectory;
            Directory.CreateDirectory(mocksDirectory);
            for (var index = 0; index < mock.Responses.Length; index++)
            {
                try
                {
                    var response = mock.Responses[index];
                    
                    var path = $"{mocksDirectory}{Path.DirectorySeparatorChar}{_mocksDirectoryByType[response.Type]}{Path.DirectorySeparatorChar}";
                    Directory.CreateDirectory(path);
                    await File.WriteAllTextAsync(
                        $"{path}{mock.Name.Replace(" ", string.Empty).ToLowerInvariant()}.json",
                        $"{{\n  \"orchestratedResponses\": [{string.Join(",\n", response.Contents)}]\n}}").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    if(progress != null)
                    {
                        var report = new ExportProgress
                        {
                            CurrentRequest = mock.Name,
                            PercentComplete = 1
                        };
                        progress.Report(report);
                    }
                }
            }
        }
    }
}
