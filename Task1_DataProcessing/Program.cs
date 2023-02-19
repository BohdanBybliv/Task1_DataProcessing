using System.Configuration;
using System.Text.Json;
using Task1_DataProcessing.Models;

namespace Task1_DataProcessing
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;

            string? folderA = appSettings.Get("folder_a");
            string? folderB = appSettings.Get("folder_b");
            string fileName = "raw_data.txt";

            List<Transform> transforms = new List<Transform>();

            using (StreamReader reader = new StreamReader(folderA + fileName))
            {
                var rows = reader.ReadToEnd().Split('\n');

                foreach (var row in rows)
                {
                    Console.WriteLine(row);

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

            using (FileStream fs = new FileStream(folderB + "output1.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(fs, transforms);
                Console.WriteLine("Data has been saved to file");
            }
        }
    }
}