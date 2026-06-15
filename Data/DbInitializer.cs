using BloodDonationApp.Models;
using System;
using System.Linq;

namespace BloodDonationApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Force recreation of database during development to ensure seeding runs cleanly
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any donors.
            if (context.Donors.Any())
            {
                return;   // DB has been seeded
            }

            string defaultHashedPassword = PasswordHasher.HashPassword("Password123!");

            var donors = new Donor[]
            {
                new Donor { Name = "John Doe", BloodType = "O+", Phone = "9876543210", Email = "john@example.com", City = "New York", LastDonationDate = DateTime.Parse("2024-01-01"), Password = defaultHashedPassword, ConfirmPassword = "Password123!" },
                new Donor { Name = "Jane Smith", BloodType = "A-", Phone = "9876543211", Email = "jane@example.com", City = "Los Angeles", LastDonationDate = DateTime.Parse("2023-11-15"), Password = defaultHashedPassword, ConfirmPassword = "Password123!" },
                new Donor { Name = "Robert Brown", BloodType = "B+", Phone = "8876543212", Email = "robert@example.com", City = "Chicago", IsAvailable = false, Password = defaultHashedPassword, ConfirmPassword = "Password123!" },
                new Donor { Name = "Alice Johnson", BloodType = "AB+", Phone = "7876543213", Email = "alice@example.com", City = "Houston", Password = defaultHashedPassword, ConfirmPassword = "Password123!" }
            };

            foreach (Donor d in donors)
            {
                context.Donors.Add(d);
            }
            context.SaveChanges();

            var requests = new BloodRequest[]
            {
                new BloodRequest { PatientName = "Mike Wilson", BloodType = "O+", Units = 2, Hospital = "City Hospital", City = "New York", ContactNumber = "9876543210", RequiredDate = DateTime.Now.AddDays(2), Status = "Pending" },
                new BloodRequest { PatientName = "Sarah Davis", BloodType = "A-", Units = 1, Hospital = "General Clinic", City = "Los Angeles", ContactNumber = "9876543211", RequiredDate = DateTime.Now.AddDays(1), Status = "Pending" }
            };

            foreach (BloodRequest r in requests)
            {
                context.BloodRequests.Add(r);
            }
            context.SaveChanges();
        }
    }
}
