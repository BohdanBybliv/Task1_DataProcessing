namespace Task1_DataProcessing.FileParsers.TxtFileParser
{
    internal interface ITxtFileParser
    {
        public Task ParseFile(string fileName);
    }
}
