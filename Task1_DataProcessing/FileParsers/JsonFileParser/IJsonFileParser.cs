using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.JsonFileParser
{
    internal interface IJsonFileParser
    {
        public Task<SaveMethodResult> SaveFileAsync(string fileName, List<Transform> transforms);
    }
}
