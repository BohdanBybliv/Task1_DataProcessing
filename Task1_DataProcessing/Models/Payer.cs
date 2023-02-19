namespace Task1_DataProcessing.Models
{
    public class Payer
    {
        public string Name { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public long AccountNumber { get; set; }
        public Payer(string name, decimal payment, DateTime date, long accountNumber)
        {
            Name = name;
            Payment = payment;
            Date = date;
            AccountNumber = accountNumber;
        }
        public Payer()
        {
            Name = string.Empty;
            Payment = 0;
            Date = DateTime.Now;
            AccountNumber = 0;
        }
    }
}
