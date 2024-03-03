using Microsoft.EntityFrameworkCore;
using WebApp.Models;


namespace WebApp.Data
{
    public partial class RentalContext : DbContext
    {
        public virtual DbSet<Car> Cars { get; set; } = null!;

        public virtual DbSet<Rental> Rentals { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public RentalContext(DbContextOptions<RentalContext> option) : base(option)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InitializeCarsFromCsv(modelBuilder, "cars.csv");
        }

        private void InitializeCarsFromCsv(ModelBuilder modelBuilder, string carsFilePath)
        {
            var carLines = System.IO.File.ReadAllLines(carsFilePath);

            foreach (var carLine in carLines)
            {
                var values = carLine.Split(',');

                if (values.Length == 5 &&
                    int.TryParse(values[0], out var id) &&
                    int.TryParse(values[3], out var year) &&
                    double.TryParse(values[4], out var pricePerDay))
                {
                    var car = new Car
                    {
                        Id = id,
                        Make = values[1],
                        Model = values[2],
                        Year = year,
                        PricePerDay = pricePerDay,
                        IsAvailable = true
                    };

                    modelBuilder.Entity<Car>().HasData(car);
                }
            }
        }
    }
}