namespace Task1_DataProcessing.Models
{
    public class FileParserMethodResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public FileParserMethodResult(bool isSuccess, string message, int parsedLines, int fountErrors = 0)
        {
            IsSuccess = isSuccess;
            Message = message;
            ParsedLines = parsedLines;
            FoundErrors = fountErrors;
        }
    }
}
