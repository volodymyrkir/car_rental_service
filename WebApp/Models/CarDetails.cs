namespace WebApp.Models
{
    public class CarDetailsViewModel
    {
        public string Make {  get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }
}
