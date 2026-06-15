using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BloodDonationApp.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        public string? RequiredRole { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("UserRole");

            // If user is not logged in, redirect to respective login portal
            if (string.IsNullOrEmpty(userRole))
            {
                if (RequiredRole == "Admin")
                {
                    context.Result = new RedirectToActionResult("AdminLogin", "Account", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("UserLogin", "Account", null);
                }
                return;
            }

            // If a specific role is required (e.g. "Admin") and user doesn't have it, redirect to respective dashboard
            if (!string.IsNullOrEmpty(RequiredRole) && userRole != RequiredRole)
            {
                if (userRole == "User")
                {
                    context.Result = new RedirectToActionResult("Dashboard", "Donor", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Admin", null);
                }
            }
        }
    }
}
