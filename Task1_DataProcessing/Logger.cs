using System.Configuration;

namespace Task1_DataProcessing
{
    public class Logger
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public List<string> InvalidFiles { get; set; }
        private readonly string? _folder_b;
        public Logger()
        {
            ParsedFiles = 0;
            ParsedLines = 0;
            FoundErrors = 0;
            InvalidFiles = new List<string>();
            _folder_b = ConfigurationManager.AppSettings.Get("folder_b");
        }
        public void SaveLog()
        {
            string path = $"{_folder_b}/{DateTime.Now.ToString("dd-MM-yyyy")}";
            Directory.CreateDirectory(path);

            string outputFile = $"{path}/meta.log";

            using (StreamWriter writer = new StreamWriter(outputFile, false))
            {
                writer.WriteLine($"parsed_files: {ParsedFiles}");
                writer.WriteLine($"parsed_lines: {ParsedLines}");
                writer.WriteLine($"found_errors: {FoundErrors}");
                writer.Write("invalid_files:");
                if (FoundErrors == 0) writer.Write(" none");
                else
                {
                    foreach (string line in InvalidFiles)
                    {
                        writer.Write($" {line},");
                    }
                }
            }
            ResetLog();
        }
        public void ResetLog()
        {
            ParsedFiles = 0;
            ParsedLines = 0;
            FoundErrors = 0;
            InvalidFiles.Clear();
        }
    }
}
