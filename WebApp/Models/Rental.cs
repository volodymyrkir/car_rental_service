namespace WebApp.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CustomerMail { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public double TotalPaid { get; set; }
    }
}
