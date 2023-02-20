using System.Configuration;
using Task1_DataProcessing.FileParsers.JsonFileParser;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.CsvFileParser
{
    internal class CsvFileParser : ICsvFileParser
    {
        private readonly IJsonFileParser _fileParser;
        private readonly Logger _logger;
        public CsvFileParser(Logger logger)
        {
            _fileParser = new JsonFileParser.JsonFileParser();
            _logger = logger;
        }
        public async Task<FileParserMethodResult> ParseFile(string fileName)
        {
            if (!File.Exists(fileName)) return new FileParserMethodResult(false, "File doesn't exist", 0);

            int lines = 0;
            int foundErrors = 0;
            List<Transform> transforms = new List<Transform>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                var input = await reader.ReadToEndAsync();
                var rows = input.Split('\n').ToList();
                rows.Remove(rows[0]);
                lines = rows.Count;

                foreach (var row in rows)
                {
                    string name;
                    string city;
                    decimal payment;
                    DateTime date;
                    long accountNumber;
                    string serviceName;
                    try
                    {
                        var items = row.Replace(",", "").Replace("\"", "").Split(';');

                        name = items[0];

                        city = items[1].Substring(0, items[1].IndexOf(' '));

                        payment = Convert.ToDecimal(items[2].Replace(".", ","));

                        date = DateTime.ParseExact(items[3], "yyyy-dd-MM", null);

                        accountNumber = Convert.ToInt64(items[4]);

                        serviceName = items[5].Replace("\r", "");
                    }
                    catch (Exception)
                    {
                        lines--;
                        foundErrors++;
                        continue;
                    }

                    var transform = transforms.FirstOrDefault(f => f.City == city);
                    Payer payer = new Payer(name, payment, date, accountNumber);

                    if (transform is null)
                    {
                        Service service = new Service();
                        service.Name = serviceName;
                        service.Payers.Add(payer);
                        service.Total += payment;

                        transform = new Transform();
                        transform.City = city;
                        transform.Services.Add(service);
                        transform.Total += payment;

                        transforms.Add(transform);
                    }
                    else
                    {
                        var service = transform.Services.FirstOrDefault(s => s.Name == serviceName);

                        if (service is null)
                        {
                            service = new Service();
                            service.Name = serviceName;
                            service.Payers.Add(payer);
                            service.Total += payment;

                            transform.Services.Add(service);
                            transform.Total += payment;
                        }
                        else
                        {
                            service.Payers.Add(payer);
                            service.Total += payment;

                            transform.Total += payment;
                        }
                    }
                }
            }

            if (lines == 0) return new FileParserMethodResult(false, "The file is empty or all lines have errors (missing values or invalid types)", lines, foundErrors);

            string outputFile = $"output{_logger.ParsedFiles + 1}.json";

            var result = await _fileParser.SaveFileAsync(outputFile, transforms);

            if (result.IsSuccess) return new FileParserMethodResult(true, $"The file is processed, the result is saved in {outputFile}!", lines, foundErrors);
            else return new FileParserMethodResult(false, $"\nThe file is processed but doesn't saved.\nException: {result.Message}", 0);
        }
    }
}
