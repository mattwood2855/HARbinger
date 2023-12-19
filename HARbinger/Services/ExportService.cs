using HARbinger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static Task ExportAllMocks(string username, IProgress<ExportProgress> progress)
        {
            var graphRequestNames = HarFileService.GetGraphRequests();
            var mocks = HarFileService.GetMockAsync(username, graphRequestNames);
            return ExportMockAsync(mocks, progress, RepoService.GetMockPath());
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
                    if (progress != null)
                    {
                        var report = new ExportProgress
                        {
                            CurrentRequest = mock.Name,
                            PercentComplete = index / mock.Responses.Length
                        };
                        progress.Report(report);
                    }
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
            }
        }
    }
}
