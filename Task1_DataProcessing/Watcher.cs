using System.Configuration;
using System.Timers;
using Task1_DataProcessing.FileParsers.CsvFileParser;
using Task1_DataProcessing.FileParsers.TxtFileParser;
using Task1_DataProcessing.Models;
using Timer = System.Timers.Timer;

namespace Task1_DataProcessing
{
    internal class Watcher : IDisposable
    {
        private readonly string? _folderA;
        private readonly FileSystemWatcher _watcher;
        private readonly ITxtFileParser _txtParser;
        private readonly ICsvFileParser _csvParser;
        private readonly Logger _logger;
        private Timer _timer;
        public Watcher()
        {
            _folderA = ConfigurationManager.AppSettings.Get("folder_a");
            _watcher = new FileSystemWatcher(_folderA);
            _logger = new Logger();
            _txtParser = new TxtFileParser(_logger);
            _csvParser = new CsvFileParser(_logger);
            _timer = new Timer();        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            _watcher.Dispose();
        }
        private void StartTimer()
        {
            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 23, 59, 0, 0); //Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (scheduledTime - DateTime.Now).TotalMilliseconds;
            _timer = new Timer(tickTime);
            _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            _timer.Start();
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            _logger.SaveLog();

            StartTimer();
        }
        public async Task Start()
        {
            Console.WriteLine("Start application...");

            StartTimer();

            await CheckFolder();

            _watcher.NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.LastAccess
                    | NotifyFilters.Size;

            _watcher.Filters.Add("*.txt");
            _watcher.Filters.Add("*.csv");

            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;

            while (true)
            {
                Console.WriteLine("\nPress 'r' to reset, 's' to stop and save log.");
                string? input = Console.ReadLine();
                if (input == "r") Reset();
                else if (input == "s")
                {
                    Stop();
                    break;
                }
            }
        }
        private async Task CheckFolder()
        {
            var txtFiles = Directory.GetFiles(_folderA, "*.txt");
            var csvFiles = Directory.GetFiles(_folderA, "*.csv");
            foreach (var file in txtFiles)
            {
                string fileName = Path.GetFileName(file);

                Console.WriteLine($"\nFound new file: {fileName}");

                var result = await _txtParser.ParseFile(file);

                UpdateLog(result, fileName);

                if (File.Exists(file)) File.Delete(file);
            }
            foreach (var file in csvFiles)
            {
                string fileName = Path.GetFileName(file);

                Console.WriteLine($"\nFound new file: {fileName}");

                var result = await _csvParser.ParseFile(file);

                UpdateLog(result, fileName);

                if (File.Exists(file)) File.Delete(file);
            }
        }
        public void Reset()
        {
            _logger.ResetLog();
            Console.WriteLine("\nLog was reset!");
        }
        public void Stop()
        {
            _logger.SaveLog();
            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
            _timer.Stop();
            _timer.Dispose();
        }
        private void UpdateLog(FileParserMethodResult result, string fileName)
        {
            Console.WriteLine(result.Message);
            _logger.ParsedFiles++;
            _logger.ParsedLines += result.ParsedLines;
            _logger.FoundErrors += result.FoundErrors;
            if (result.FoundErrors > 0) _logger.InvalidFiles.Add(fileName);
        }
        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            Console.WriteLine($"\nAdded new file: {Path.GetFileName(path)}");

            FileParserMethodResult result;
            if (Path.GetExtension(path) == ".txt") result = await _txtParser.ParseFile(path);
            else result = await _csvParser.ParseFile(path);

            UpdateLog(result, Path.GetFileName(path));

            if (File.Exists(path)) File.Delete(path);
        }
    }
}
