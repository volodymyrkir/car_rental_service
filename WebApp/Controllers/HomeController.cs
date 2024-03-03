using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult GoToCarIndex()
        {
            return RedirectToAction("Index", "Car");
        }

        public IActionResult GoToRentals()
        {
            return RedirectToAction("Index", "Rental");
        }
        public IActionResult GoToCustomers()
        {
            return RedirectToAction("Index", "Customer");
        }
    }
}
