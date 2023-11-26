namespace WebApp.Models
{
    public class CarCheckoutViewModel
    {
        public int CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public double PricePerDay { get; set; }
    }
}
