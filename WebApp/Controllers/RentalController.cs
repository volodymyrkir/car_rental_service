using Microsoft.AspNetCore.Mvc;

using WebApp.Data;

namespace WebApp.Controllers
{
    public class RentalController : Controller
    {
        private readonly RentalContext _context;

        public RentalController(RentalContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var rentalToDelete = await _context.Rentals.FindAsync(id);
                
                if (rentalToDelete == null)
                {
                    return Json(new { success = false, message = "Rental not found" });
                }

                var rentalCar = await _context.Cars.FindAsync(rentalToDelete.CarId);
                rentalCar.IsAvailable = true;
                _context.Rentals.Remove(rentalToDelete);
                await _context.SaveChangesAsync();

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
            var rentals = _context.Rentals.ToList();
            return View(rentals);
        }

    }
}