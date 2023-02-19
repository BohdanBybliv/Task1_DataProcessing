using System.Configuration;
using Task1_DataProcessing.FileParsers.CsvFileParser;
using Task1_DataProcessing.FileParsers.TxtFileParser;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing
{
    internal class Watcher : IDisposable
    {
        private readonly string? _folderA;
        private readonly FileSystemWatcher _watcher;
        private readonly ITxtFileParser _txtParser;
        private readonly ICsvFileParser _csvParser;
        private readonly Logger _logger;
        public Watcher()
        {
            _folderA = ConfigurationManager.AppSettings.Get("folder_a");
            _watcher = new FileSystemWatcher(_folderA);
            _logger = new Logger();
            _txtParser = new TxtFileParser(_logger);
            _csvParser = new CsvFileParser(_logger);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            _watcher.Dispose();
        }
        public void Start()
        {
            Console.WriteLine("Start application...");

            _watcher.NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.LastAccess
                    | NotifyFilters.Size;

            _watcher.Filters.Add("*.txt");
            _watcher.Filters.Add("*.csv");

            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }
        public void Reset()
        {
            _logger.ResetLog();
        }
        public void Stop()
        {
            _logger.SaveLog();
            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
        }
        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            Console.WriteLine($"Added new file: {e.Name}");

            FileParserMethodResult result;
            if (Path.GetExtension(path) == ".txt") result = await _txtParser.ParseFile(path);
            else result = await _csvParser.ParseFile(path);

            if (result.IsSuccess)
            {
                Console.WriteLine(result.Message);
                _logger.ParsedFiles++;
                _logger.ParsedLines += result.ParsedLines;
            }
            else
            {
                Console.WriteLine(result.Message);
                _logger.ParsedFiles++;
                _logger.ParsedLines += result.ParsedLines;
                _logger.FoundErrors += result.FoundErrors;
                _logger.InvalidFiles.Add(e.Name);
            }
        }
    }
}
