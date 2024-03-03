using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly RentalContext _context;

        public CustomerController(RentalContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string email)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(email);
                
                if (customer == null)
                {
                    return Json(new { success = false, message = "Customer not found" });
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "customer deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return Json(new { success = false, message = "Error deleting customer" });
            }
        }

        [HttpGet]
        public IActionResult Profile(string email)
        {
            var customerRentals = _context.Rentals.Where(r => r.CustomerMail == email).ToList();
            var totalAmountPaid = customerRentals.Sum(r => r.TotalPaid);

            var viewModel = new CustomerProfileViewModel
            {
                CustomerEmail = email,
                Rentals = customerRentals,
                TotalAmountPaid = totalAmountPaid
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

    }
}