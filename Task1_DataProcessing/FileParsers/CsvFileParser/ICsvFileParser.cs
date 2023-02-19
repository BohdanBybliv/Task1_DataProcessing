using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.CsvFileParser
{
    internal interface ICsvFileParser
    {
        public Task<FileParserMethodResult> ParseFile(string fileName);
    }
}
