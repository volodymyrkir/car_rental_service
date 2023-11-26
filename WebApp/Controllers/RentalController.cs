using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class RentalController : BaseController
    {
        private List<Rental> _rentals;
         

        public RentalController()
        {
           UpdateData();
            _rentals = LoadRentsFromCsv("rents.csv");
        }
        
        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                string filePath = "rents.csv";
                var lines = System.IO.File.ReadAllLines(filePath);

                var updatedLines = lines.Where(line =>
                {
                    var values = line.Split(',');
                    return values.Length >= 1 && int.TryParse(values[0], out var currentId) && currentId != id;
                });
                SaveRentsToCsv(updatedLines, filePath);
                return Json(new { success = true, message = "Rental deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting rental: {ex.Message}");
                return Json(new { success = false, message = "Error deleting rental" });
            }
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(_rentals);
        }

    }
}
