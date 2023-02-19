using System.Configuration;
using Task1_DataProcessing.FileParsers.JsonFileParser;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.CsvFileParser
{
    internal class CsvFileParser : ICsvFileParser
    {
        private readonly IJsonFileParser _fileParser;
        private readonly string? _folder_b;
        public CsvFileParser()
        {
            _fileParser = new JsonFileParser.JsonFileParser();
            _folder_b = ConfigurationManager.AppSettings.Get("folder_b");
        }
        public async Task ParseFile(string fileName)
        {
            List<Transform> transforms = new List<Transform>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                var input = await reader.ReadToEndAsync();
                var rows = input.Split('\n');

                foreach (var row in rows)
                {
                    var items = row.Replace(",", "").Replace("\"", "").Split(';');

                    string name = items[0];
                    
                    string city = items[1].Substring(0, items[1].IndexOf(' '));
                    
                    decimal payment = Convert.ToDecimal(items[2].Replace(".", ","));
                    
                    DateTime date = DateTime.ParseExact(items[3], "yyyy-dd-MM", null);

                    long accountNumber = Convert.ToInt64(items[4]);

                    string serviceName = items[5].Replace("\r", "");

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

            await _fileParser.SaveFileAsync(_folder_b + "output2.json", transforms);
        }
    }
}
