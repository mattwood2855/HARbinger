using LibGit2Sharp;

namespace HARbinger.Services
{
    public class RepoData
    {
        public string Branch { get; }
        public string Name { get; }

        public RepoData(Repository repo)
        {
            Branch = repo.Head.FriendlyName;
            Name = repo.Info.WorkingDirectory;
        }
    }

    public static class RepoService
    {
        public static string Path { get; private set; }
        public static Repository Repository { get; private set; }

        public static string GetMockPath()
        {
            return Path + "/mock_server/src/mock_files/graph";
        }

        public static RepoData? GetRepoDataForPath(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if(!Repository.IsValid(path))
            {
                return null;
            }

            try
            {
                Path = path;
                Repository = new Repository(path);
                var repoData = new RepoData(Repository);
                return repoData;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
