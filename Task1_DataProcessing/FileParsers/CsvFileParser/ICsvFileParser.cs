namespace Task1_DataProcessing.FileParsers.CsvFileParser
{
    internal interface ICsvFileParser
    {
        public Task ParseFile(string fileName);
    }
}
