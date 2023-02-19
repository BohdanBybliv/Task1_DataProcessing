namespace Task1_DataProcessing.Models
{
    public class Service
    {
        public string Name { get; set; }
        public List<Payer> Payers { get; set; }
        public decimal Total { get; set; }
        public Service()
        {
            Name = string.Empty;
            Payers = new List<Payer>();
            Total = 0;
        }
        public Service(string name, List<Payer> payers, decimal total)
        {
            Name = name;
            Payers = payers;
            Total = total;
        }
    }
}
