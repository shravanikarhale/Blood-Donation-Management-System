using Microsoft.AspNetCore.Mvc;
using BloodDonationApp.Data;
using BloodDonationApp.Models;
using Microsoft.EntityFrameworkCore;
using BloodDonationApp.Filters;

namespace BloodDonationApp.Controllers
{
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donor/Register (Public)
        public IActionResult Register()
        {
            return View();
        }

        // POST: Donor/Register (Public)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,BloodType,Phone,Email,City,LastDonationDate,Password,ConfirmPassword")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate email to ensure data integrity
                    bool emailExists = await _context.Donors.AnyAsync(d => d.Email.ToLower() == donor.Email.ToLower());
                    if (emailExists)
                    {
                        // Add model error if email is already registered
                        ModelState.AddModelError("Email", "A donor with this email address is already registered.");
                        return View(donor);
                    }

                    donor.Password = PasswordHasher.HashPassword(donor.Password);
                    donor.IsAvailable = true; // Default to available on registration
                    // Save the new donor to the database
                    _context.Add(donor);
                    await _context.SaveChangesAsync();
                    
                    // Redirect to a confirmation page on success (PRG pattern)
                    return RedirectToAction(nameof(RegistrationSuccess));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An unexpected error occurred while processing your registration. Please try again later.");
                }
            }
            
            return View(donor);
        }

        // GET: Donor/RegistrationSuccess (Public)
        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        // GET: Donor/Dashboard (Secured)
        [AuthFilter]
        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("UserLogin", "Account");
            }

            var donor = await _context.Donors.FindAsync(userId);
            if (donor == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("UserLogin", "Account");
            }

            return View(donor);
        }

        // GET: Donor/Profile (Secured)
        [AuthFilter]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("UserLogin", "Account");
            }

            var donor = await _context.Donors.FindAsync(userId);
            if (donor == null) return NotFound();

            return View(donor);
        }

        // POST: Donor/Profile (Secured)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthFilter]
        public async Task<IActionResult> Profile(int id, [Bind("Id,Name,BloodType,Phone,Email,City,LastDonationDate,IsAvailable")] Donor donor)
        {
            if (id != donor.Id) return NotFound();

            var existingDonor = await _context.Donors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (existingDonor == null) return NotFound();

            donor.Password = existingDonor.Password;
            donor.ConfirmPassword = existingDonor.Password;

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donor);
                    await _context.SaveChangesAsync();
                    
                    // Update session variables in case details changed
                    HttpContext.Session.SetString("UserName", donor.Name);
                    HttpContext.Session.SetString("UserEmail", donor.Email);
                    HttpContext.Session.SetString("UserBloodType", donor.BloodType);

                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction(nameof(Dashboard));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Donors.Any(e => e.Id == donor.Id)) return NotFound();
                    else throw;
                }
            }
            return View(donor);
        }

        // GET: Donor/Donate (Secured)
        [AuthFilter]
        public async Task<IActionResult> Donate()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("UserLogin", "Account");
            }

            var donor = await _context.Donors.FindAsync(userId);
            if (donor == null) return NotFound();

            return View(donor);
        }

        // POST: Donor/Donate (Secured toggle)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthFilter]
        public async Task<IActionResult> Donate(int id, bool isAvailable)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return NotFound();

            donor.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isAvailable 
                ? "You have marked yourself as AVAILABLE to donate blood." 
                : "You have marked yourself as BUSY/UNAVAILABLE.";

            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Donor/History (Secured)
        [AuthFilter]
        public async Task<IActionResult> History()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("UserLogin", "Account");
            }

            var donor = await _context.Donors.FindAsync(userId);
            if (donor == null) return NotFound();

            // Simulate donation records based on LastDonationDate or static entries
            return View(donor);
        }

        // GET: Donor/Search
        [AuthFilter]
        public async Task<IActionResult> Search(string bloodType, string city)
        {
            var donors = from d in _context.Donors
                         where d.IsAvailable
                         select d;

            if (!string.IsNullOrEmpty(bloodType))
            {
                // Fix the '+' URL query decoding bug: '+' becomes a space in URLs
                string normalizedBloodType = bloodType.Replace(" ", "+").Trim().ToUpper();
                donors = donors.Where(s => s.BloodType.ToUpper() == normalizedBloodType);
            }

            if (!string.IsNullOrEmpty(city))
            {
                string normalizedCity = city.Trim().ToUpper();
                donors = donors.Where(s => s.City.ToUpper().Contains(normalizedCity));
            }

            return View(await donors.ToListAsync());
        }
    }
}
