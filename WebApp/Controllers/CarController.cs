using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CarController : BaseController
    {
        private List<Car> _cars;
        private readonly HttpClient _httpClient;
        

        public CarController(IHttpClientFactory httpClientFactory)
        {
            UpdateData();
            _httpClient = httpClientFactory.CreateClient();
            _cars = LoadCarsFromCsv("cars.csv", "rents.csv");
        {
            
            
        };
        }
        private List<Car> LoadCarsFromCsv(string carsFilePath, string rentsFilePath)
        {
            var cars = new List<Car>();

            try
            {
                
                var carLines = System.IO.File.ReadAllLines(carsFilePath);

             
                var rentedCarIds = new HashSet<int>();

                
                if (System.IO.File.Exists(rentsFilePath))
                {
                    var rentLines = System.IO.File.ReadAllLines(rentsFilePath);
                    foreach (var rentLine in rentLines)
                    {
                        var rentValues = rentLine.Split(',');

                        if (rentValues.Length >= 2 && int.TryParse(rentValues[1], out var rentedCarId))
                        {
                            rentedCarIds.Add(rentedCarId);
                        }
                    }
                }

              
                foreach (var carLine in carLines)
                {
                    var values = carLine.Split(',');

                    if (values.Length == 5 &&
                        int.TryParse(values[0], out var id) &&
                        int.TryParse(values[3], out var year) &&
                        double.TryParse(values[4], out var pricePerDay))
                    {
                        cars.Add(new Car
                        {
                            Id = id,
                            Make = values[1],
                            Model = values[2],
                            Year = year,
                            PricePerDay = pricePerDay,
                            IsAvailable = !rentedCarIds.Contains(id)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return cars;
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

        [HttpPost]
        public IActionResult SubmitCheckout(string email, int carId, int days, decimal PricePerDay)
        {
            try
            {
                var filePath = "rents.csv";
                var lines = System.IO.File.ReadAllLines(filePath);

                int rentId = lines.Length > 1 ? int.Parse(lines.Last().Split(',')[0]) + 1 : 1;
                decimal totalPaid = PricePerDay * days;

                string rentStartDate = DateTime.Now.ToString("dd.MM.yyyy");
                string rentEndDate = DateTime.Now.AddDays(days).ToString("dd.MM.yyyy");
                var newLine = $"{rentId},{carId},{email},{rentStartDate},{rentEndDate},{totalPaid}";
                System.IO.File.AppendAllLines(filePath, new[] { newLine });

                Console.WriteLine($"Rent added to CSV: {newLine}");
                TempData["RentAdded"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting checkout: {ex.Message}");
                return RedirectToAction("Index");
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
