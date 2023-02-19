namespace Task1_DataProcessing.Models
{
    public class Transform
    {
        public string City { get; set; }
        public List<Service> Services { get; set; }
        public decimal Total { get; set; }
        public Transform()
        {
            City = string.Empty;
            Services = new List<Service>();
            Total = 0;
        }
        public Transform(string city, List<Service> services, decimal total)
        {
            City = city;
            Services = services;
            Total = total;
        }
    }
}
