using System.Text.Json;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.JsonFileParser
{
    public class JsonFileParser : IJsonFileParser
    {
        public async Task SaveFileAsync(string path, List<Transform> transforms)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(fs, transforms);
            }
        }
    }
}
