using Microsoft.AspNetCore.Mvc;
using BloodDonationApp.Data;
using System.Linq;

namespace BloodDonationApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Redirect default login to the landing page role selection
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin") return RedirectToAction("Index", "Admin");
            if (role == "User") return RedirectToAction("Dashboard", "Donor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            if (email.ToLower() == "admin@blood.org" && password == "admin123")
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserName", "Administrator");
                HttpContext.Session.SetString("UserEmail", email);
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Invalid Admin credentials.");
            return View();
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin") return RedirectToAction("Index", "Admin");
            if (role == "User") return RedirectToAction("Dashboard", "Donor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            // Query donor by Email and hashed Password
            string hashedPassword = PasswordHasher.HashPassword(password);
            var donor = _context.Donors.FirstOrDefault(d => d.Email.ToLower() == email.ToLower() && d.Password == hashedPassword);
            if (donor != null)
            {
                HttpContext.Session.SetString("UserRole", "User");
                HttpContext.Session.SetString("UserName", donor.Name);
                HttpContext.Session.SetString("UserEmail", donor.Email);
                HttpContext.Session.SetString("UserId", donor.Id.ToString());
                HttpContext.Session.SetString("UserBloodType", donor.BloodType);
                return RedirectToAction("Dashboard", "Donor");
            }

            ModelState.AddModelError("", "Invalid credentials. Please enter your registered Email and Password.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction("Index", "Home");
        }
    }
}
