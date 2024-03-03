using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CarController : Controller
    {
        private readonly RentalContext _context;
        private readonly HttpClient _httpClient;
        private List<Car> _cars;

        public CarController(RentalContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();

            _cars = _context.Cars.ToList();
        }

        public IActionResult Rent(int carId)
        {
            var selectedCar = _cars.FirstOrDefault(car => car.Id == carId);

            if (selectedCar != null)
            {
                var checkoutViewModel = new CarCheckoutViewModel
                {
                    CarId = carId,
                    Make = selectedCar.Make,
                    Model = selectedCar.Model,
                    Year = selectedCar.Year,
                    PricePerDay = selectedCar.PricePerDay
                };

                return View("Checkout", checkoutViewModel);
            }

            TempData["ErrorMessage"] = "Selected car not found.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(string make, string model, int year)
        {
            var details = GetCarDetailsFromApiAsync(make, model, year).Result;

            if (details != null)
            {
                var carDetailsViewModel = new CarDetailsViewModel
                {
                    Make = make,
                    Model = model,
                    Year = year,
                    Details = details
                };

                return View(carDetailsViewModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Details for this car are not available.";
                return RedirectToAction("Index");
            }
        }

        private async Task<Dictionary<string, object>?> GetCarDetailsFromApiAsync(string make, string model, int year)
        {
            var apiUrl = $"https://www.carqueryapi.com/api/0.3/?cmd=getTrims&make={make}&model={model}&year={year}";
            var apiResponse = await _httpClient.GetStringAsync(apiUrl);
            var responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiResponse);

            if (responseObj.TryGetValue("Trims", out var trims) && trims is JArray trimsArray)
            {
                var firstTrim = trimsArray.FirstOrDefault();

                return firstTrim?.ToObject<Dictionary<string, object>>();
            }

            return null;
        }

        [HttpPost]
        public JsonResult CheckDetails(string make, string model, int year)
        {
            var details = GetCarDetailsFromApiAsync(make, model, year).Result;
            return Json(details != null && details.Count != 0);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCheckout(string email, int carId, string fullname, int days, decimal PricePerDay)
        {
            try
            {
        
                var existingUser = _context.Customers.FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    var newUser = new Customer
                    {
                        Email = email,
                        FullName = fullname,
                    };

                    _context.Customers.Add(newUser);
                    await _context.SaveChangesAsync();
                }

                var selectedCar = _cars.FirstOrDefault(car => car.Id == carId);
                if (selectedCar == null)
                {
                    TempData["ErrorMessage"] = "Selected car not found.";
                    return RedirectToAction("Index");
                }

                decimal totalPaid = PricePerDay * days;

                string rentStartDate = DateTime.Now.ToString("yyyy-MM-dd");
                string rentEndDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");

                var rental = new Rental
                {
                    CarId = carId,
                    CustomerMail = email,
                    RentStartDate = DateTime.Parse(rentStartDate),
                    RentEndDate = DateTime.Parse(rentEndDate),
                    TotalPaid = (double)totalPaid
                };

                _context.Rentals.Add(rental);
                selectedCar.IsAvailable = false;
                await _context.SaveChangesAsync();

                TempData["RentAdded"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting checkout: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);

                if (car == null)
                {
                    return Json(new { success = false, message = "Car not found" });
                }
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Car deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Car: {ex.Message}");
                return Json(new { success = false, message = "Error deleting Car" });
            }
        }
        [HttpPost]
        public IActionResult Add(string make, string model, int year, string pricePerDay)
        {
            try
            {
                if (double.TryParse(pricePerDay, NumberStyles.Number, CultureInfo.InvariantCulture, out double parsedPricePerDay))
                {
                    var newCar = new Car
                    {
                        Make = make,
                        Model = model,
                        Year = year,
                        PricePerDay = parsedPricePerDay,
                        IsAvailable = true
                    };

                    _context.Cars.Add(newCar);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Car added successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid price per day format" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new car: {ex.Message}");
                return Json(new { success = false, message = "Error adding new car" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_cars);
        }
    }
}