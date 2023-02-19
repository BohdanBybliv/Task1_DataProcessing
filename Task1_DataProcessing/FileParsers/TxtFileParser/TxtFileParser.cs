﻿using System.Configuration;
using Task1_DataProcessing.FileParsers.JsonFileParser;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing.FileParsers.TxtFileParser
{
    public class TxtFileParser : ITxtFileParser
    {
        private readonly IJsonFileParser _fileParser;
        private readonly string? _folder_b;
        public TxtFileParser()
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
                    var items = row.Replace(",", "").Split('"');

                    string name = items[0].Remove(items[0].Length - 1);

                    string city = items[1].Substring(0, items[1].IndexOf(' '));

                    decimal payment;
                    DateTime date;
                    long accountNumber;
                    string serviceName;

                    if (items.Length == 3)
                    {
                        var values = items[2].Split(' ');

                        payment = Convert.ToDecimal(values[1].Replace(".", ","));

                        date = DateTime.ParseExact(values[2], "yyyy-dd-MM", null);

                        accountNumber = Convert.ToInt64(values[3]);

                        serviceName = values[4].Replace(" ", "").Replace("\r", "");
                    }
                    else
                    {
                        var values = items[2].Split(' ');

                        payment = Convert.ToDecimal(values[1].Replace(".", ","));

                        date = DateTime.ParseExact(values[2], "yyyy-dd-MM", null);

                        accountNumber = Convert.ToInt64(items[3]);

                        serviceName = items[4].Replace(" ", "").Replace("\r", "");
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

            await _fileParser.SaveFileAsync(_folder_b + "output1.json", transforms);
        }
    }
}
