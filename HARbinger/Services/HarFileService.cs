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
    }
}
