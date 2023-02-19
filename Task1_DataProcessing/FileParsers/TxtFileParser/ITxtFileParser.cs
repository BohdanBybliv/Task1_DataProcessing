using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.TxtFileParser
{
    internal interface ITxtFileParser
    {
        public Task<FileParserMethodResult> ParseFile(string fileName);
    }
}
