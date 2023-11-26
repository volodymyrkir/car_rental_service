using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected void UpdateData()
        {
            var rentsFilePath = "rents.csv";
            var rents = LoadRentsFromCsv(rentsFilePath);

            var currentDate = DateTime.Now;

            var outdatedRents = rents.Where(r => r.RentEndDate < currentDate).ToList();

            foreach (var outdatedRent in outdatedRents)
            {
                rents.Remove(outdatedRent);
            }
            
            var updatedLines = rents.Select(rent =>
                $"{rent.Id},{rent.CarId},{rent.CustomerMail},{rent.RentStartDate},{rent.RentEndDate},{rent.TotalPaid}");

            SaveRentsToCsv(updatedLines, rentsFilePath);
        }

        protected List<Rental> LoadRentsFromCsv(string filePath)
        {
            var rents = new List<Rental>();

            try
            {
                var lines = System.IO.File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var values = line.Split(',');

                    if (values.Length == 6 &&
                        int.TryParse(values[0], out var id) &&
                        int.TryParse(values[1], out var carId) &&
                        DateTime.TryParse(values[3], out var rentStartDate) &&
                        DateTime.TryParse(values[4], out var rentEndDate) &&
                        double.TryParse(values[5], out var totalPaid))
                    {
                        rents.Add(new Rental
                        {
                            Id = id,
                            CarId = carId,
                            CustomerMail = values[2],
                            RentStartDate = rentStartDate,
                            RentEndDate = rentEndDate,
                            TotalPaid = totalPaid,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return rents;
        }

        protected void SaveRentsToCsv(IEnumerable<string> updatedLines, string filePath)
        {
            System.IO.File.WriteAllLines(filePath, updatedLines);
        }
    }
}