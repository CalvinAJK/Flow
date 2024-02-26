using Flow.Data;
using Flow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FlowContext _context;
        private const string SessionOrganizationId = "_OrganizationId";

        public HomeController(ILogger<HomeController> logger, FlowContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Check if the user is logged in
            if (User.Identity.IsAuthenticated)
            {
                // Retrieve the user's ID from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Check if the user has an existing organization selection in the session
                var organizationId = HttpContext.Session.GetInt32(SessionOrganizationId);

                // If the user doesn't have an existing organization selection, select the first organization from the database
                if (organizationId == null)
                {
                    var firstOrganization = _context.Organizations.FirstOrDefault();
                    if (firstOrganization != null)
                    {
                        HttpContext.Session.SetInt32(SessionOrganizationId, firstOrganization.Id);
                        SetUserRole(userId, firstOrganization.Id);
                    }
                }
                else
                {
                    // Retrieve the user's role based on the organization
                    SetUserRole(userId, organizationId.Value);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangeOrganization(int organizationId)
        {
            // Update the session with the selected organization ID
            HttpContext.Session.SetInt32(SessionOrganizationId, organizationId);

            // Retrieve user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Set the user's role based on the selected organization
            SetUserRole(userId, organizationId);

            // Return JSON indicating success and redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
        }

        private void SetUserRole(string userId, int organizationId)
        {
            var userRole = _context.OrganizationRoles
                .Include(or => or.Organization)
                .FirstOrDefault(or => or.UserId == userId && or.OrganizationId == organizationId);

            if (userRole != null)
            {
                // Set the user's role in the session or claims
                HttpContext.Session.SetString("UserRole", userRole.Role);
                // Set the organization ID and user role in ViewBag to access them in the view
                ViewBag.OrganizationId = organizationId;
                ViewBag.UserRole = userRole.Role;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
