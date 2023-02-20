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
            LoadLog();
        }
        public void SaveLog()
        {
            string path = $"{_folder_b}/{DateTime.Now.ToString("dd-MM-yyyy")}";
            Directory.CreateDirectory(path);

            string outputFile = $"{path}/meta.log";

            using (StreamWriter writer = new StreamWriter(outputFile))
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
            Console.WriteLine("\nLog is saved!");
        }
        public void LoadLog()
        {
            string path = $"{_folder_b}/{DateTime.Now.ToString("dd-MM-yyyy")}/meta.log";
            
            if (!File.Exists(path)) return;
            
            using (StreamReader reader = new StreamReader(path))
            {
                var rows = reader.ReadToEnd().Split('\n');

                if (rows.Length < 4) return;

                try
                {
                    ParsedFiles += Convert.ToInt32(rows[0].Split(' ')[1]);

                    ParsedLines += Convert.ToInt32(rows[1].Split(' ')[1]);

                    int foundErrors = Convert.ToInt32(rows[2].Split(' ')[1]);
                    FoundErrors += foundErrors;

                    if (foundErrors != 0)
                    {
                        var invalidFiles = rows[3].Replace(",", "").Split(' ');
                        for (int i = 1; i < invalidFiles.Length; i++)
                        {
                            InvalidFiles.Add(invalidFiles[i]);
                        }
                    }
                }
                catch (Exception)
                {
                    ResetLog();
                }
            }
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
