namespace Task1_DataProcessing.Models
{
    public class SaveMethodResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public SaveMethodResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
