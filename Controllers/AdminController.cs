using Microsoft.AspNetCore.Mvc;
using BloodDonationApp.Data;
using BloodDonationApp.Models;
using Microsoft.EntityFrameworkCore;
using BloodDonationApp.Filters;

namespace BloodDonationApp.Controllers
{
    [AuthFilter(RequiredRole = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Index (Dashboard Landing)
        public async Task<IActionResult> Index()
        {
            var donorsCount = await _context.Donors.CountAsync();
            var requests = await _context.BloodRequests.ToListAsync();

            ViewBag.EmergencyCount = requests.Count(r => r.Status == "Emergency");
            ViewBag.ApprovedCount = requests.Count(r => r.Status == "Approved" || r.Status == "Fulfilled");
            ViewBag.PendingCount = requests.Count(r => r.Status == "Pending");
            ViewBag.TotalDonors = donorsCount;

            // Pass recent activities/requests to dashboard
            var viewModel = new AdminDashboardViewModel
            {
                Donors = await _context.Donors.Take(5).ToListAsync(),
                Requests = requests.OrderByDescending(r => r.RequiredDate).Take(5).ToList()
            };

            return View(viewModel);
        }

        // GET: Admin/BloodRequests
        public async Task<IActionResult> BloodRequests()
        {
            var requests = await _context.BloodRequests.OrderByDescending(r => r.RequiredDate).ToListAsync();
            return View(requests);
        }

        // POST: Admin/ApproveRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Approved";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Blood request for {request.PatientName} has been approved.";
            return RedirectToAction(nameof(BloodRequests));
        }

        // POST: Admin/RejectRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Blood request for {request.PatientName} has been rejected.";
            return RedirectToAction(nameof(BloodRequests));
        }

        // POST: Admin/MakeEmergency
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeEmergency(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Emergency";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Blood request for {request.PatientName} marked as Emergency!";
            return RedirectToAction(nameof(BloodRequests));
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Donors.ToListAsync();
            return View(users);
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var donors = await _context.Donors.ToListAsync();
            var requests = await _context.BloodRequests.ToListAsync();

            ViewBag.TotalDonors = donors.Count;
            ViewBag.AvailableDonors = donors.Count(d => d.IsAvailable);
            ViewBag.TotalRequests = requests.Count;
            ViewBag.PendingRequests = requests.Count(r => r.Status == "Pending");
            ViewBag.ApprovedRequests = requests.Count(r => r.Status == "Approved");
            ViewBag.EmergencyRequests = requests.Count(r => r.Status == "Emergency");
            ViewBag.RejectedRequests = requests.Count(r => r.Status == "Rejected");

            // Group donors by BloodType for report
            var bloodTypeStats = donors.GroupBy(d => d.BloodType)
                                       .Select(g => new { BloodType = g.Key, Count = g.Count() })
                                       .ToDictionary(k => k.BloodType, v => v.Count);
            ViewBag.BloodTypeStats = bloodTypeStats;

            return View();
        }

        // GET: Admin/Feedback
        public IActionResult Feedback()
        {
            return View();
        }

        // GET: Admin/ManageDonors
        public async Task<IActionResult> ManageDonors()
        {
            var donors = await _context.Donors.ToListAsync();
            return View(donors);
        }

        // GET: Admin/EditDonor
        public async Task<IActionResult> EditDonor(int? id)
        {
            if (id == null) return NotFound();
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return NotFound();
            return View(donor);
        }

        // POST: Admin/EditDonor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDonor(int id, [Bind("Id,Name,BloodType,Phone,Email,City,LastDonationDate,IsAvailable")] Donor donor)
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
                _context.Update(donor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Donor updated successfully.";
                return RedirectToAction(nameof(ManageDonors));
            }
            return View(donor);
        }

        // POST: Admin/DeleteDonor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return NotFound();

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Donor profile deleted.";
            return RedirectToAction(nameof(ManageDonors));
        }

        // GET: Admin/Settings
        public IActionResult Settings()
        {
            return View();
        }
    }

    public class AdminDashboardViewModel
    {
        public List<Donor> Donors { get; set; } = new();
        public List<BloodRequest> Requests { get; set; } = new();
    }
}
