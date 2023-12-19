using HARbinger.Models;

namespace HARbinger.Logic
{
    internal static class FileExportLogic
    {
        private static readonly string _mocksDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}code{Path.DirectorySeparatorChar}spectrum-web{Path.DirectorySeparatorChar}mock_server{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}mock_files{Path.DirectorySeparatorChar}graph";

        private static readonly Dictionary<MockResponseType, string> _mocksDirectoryByType = new()
        {
            {  MockResponseType.Mutation, "mutation" },
            {  MockResponseType.Query, "query" }
        };

        internal static async Task ExportMocksAsync(Mock[] mocks, string mocksDirectory = null)
        {
            mocksDirectory ??= _mocksDirectory;
            foreach (var mock in mocks)
                foreach (var response in mock.Responses)
                    await File.WriteAllLinesAsync(
                        $"{mocksDirectory}{Path.DirectorySeparatorChar}{_mocksDirectoryByType[response.Type]}{Path.DirectorySeparatorChar}{mock.Name.Replace(" ", string.Empty).ToLowerInvariant()}.json",
                        response.Contents).ConfigureAwait(false);
        }
    }
}
