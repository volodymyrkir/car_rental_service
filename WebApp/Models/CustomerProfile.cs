namespace WebApp.Models
{
    public class CustomerProfileViewModel
    {
        public string CustomerEmail { get; set; }
        public List<Rental> Rentals { get; set; }
        public double TotalAmountPaid { get; set; }
    }
}
