using Microsoft.AspNetCore.Mvc;
using BloodDonationApp.Data;
using BloodDonationApp.Models;
using Microsoft.EntityFrameworkCore;
using BloodDonationApp.Filters;

namespace BloodDonationApp.Controllers
{
    [AuthFilter]
    public class BloodRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BloodRequest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BloodRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientName,BloodType,Units,Hospital,City,ContactNumber,RequiredDate")] BloodRequest bloodRequest)
        {
            if (ModelState.IsValid)
            {
                bloodRequest.Status = "Pending"; // Always default to Pending
                _context.Add(bloodRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your blood request has been successfully submitted and is awaiting administrator review.";
                return RedirectToAction("Dashboard", "Donor");
            }
            return View(bloodRequest);
        }

        // GET: BloodRequest/List
        public async Task<IActionResult> List()
        {
            return View(await _context.BloodRequests.ToListAsync());
        }
    }
}
