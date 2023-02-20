using System.Configuration;
using System.Text.Json;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.JsonFileParser
{
    public class JsonFileParser : IJsonFileParser
    {
        private readonly string? _folder_b;
        public JsonFileParser()
        {
            _folder_b = ConfigurationManager.AppSettings.Get("folder_b");
        }
        public async Task<SaveMethodResult> SaveFileAsync(string fileName, List<Transform> transforms)
        {
            try
            {
                string path = $"{_folder_b}/{DateTime.Now.ToString("dd-MM-yyyy")}";
                Directory.CreateDirectory(path);

                using (FileStream fs = new FileStream($"{path}/{fileName}", FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fs, transforms);
                }
            }
            catch (Exception ex)
            {
                return new SaveMethodResult(false, ex.Message);
            }
            
            return new SaveMethodResult(true, string.Empty);
        }
    }
}
