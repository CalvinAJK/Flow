using Flow.Data;
using Flow.Models;
using Microsoft.AspNetCore.Identity;
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

        public async Task<IActionResult> Index()
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
                    // Find the first organization that the user belongs to based on the OrganizationRole table
                    var firstOrganization = await _context.OrganizationRoles
                        .Where(or => or.UserId == userId)
                        .Select(or => or.Organization)
                        .FirstOrDefaultAsync();

                    if (firstOrganization != null)
                    {
                        HttpContext.Session.SetInt32(SessionOrganizationId, firstOrganization.Id);
                        SetUserRole(userId, firstOrganization.Id);

                        // Query the OrganizationRole entities to get all user IDs associated with the organization
                        var userIdsInOrganization = _context.OrganizationRoles
                            .Where(or => or.OrganizationId == firstOrganization.Id)
                            .Select(or => or.UserId)
                            .ToList();

                        // Query the ApplicationUser entities to get the corresponding users based on the user IDs
                        var usersInOrganization = await _context.Users
                            .Where(u => userIdsInOrganization.Contains(u.Id))
                            .ToListAsync();

                        // Retrieve the organization roles associated with the first organization
                        var organizationRoles = await _context.OrganizationRoles
                            .Where(or => or.OrganizationId == firstOrganization.Id)
                            .ToListAsync();

                        // Pass organizationRoles to the view along with usersInOrganization
                        ViewBag.OrganizationRoles = organizationRoles;
                        return View(usersInOrganization);
                    }
                }
                else if (organizationId != null) 
                {
                    var selectedOrganization = _context.Organizations.FirstOrDefault(or => or.Id == organizationId);
                    if (selectedOrganization != null)
                    {
                        // Retrieve the user's role based on the organization
                        SetUserRole(userId, organizationId.Value);

                        // Query the OrganizationRole entities to get all user IDs associated with the organization
                        var userIdsInOrganization = await _context.OrganizationRoles
                            .Where(or => or.OrganizationId == organizationId.Value)
                            .Select(or => or.UserId)
                            .ToListAsync();

                        // Query the ApplicationUser entities to get the corresponding users based on the user IDs
                        var usersInOrganization = await _context.Users
                            .Where(u => userIdsInOrganization.Contains(u.Id))
                            .ToListAsync();

                        // Retrieve the organization roles associated with the selected organization
                        var organizationRoles = await _context.OrganizationRoles
                            .Where(or => or.OrganizationId == organizationId.Value)
                            .ToListAsync();

                        // Pass organizationRoles to the view along with usersInOrganization
                        ViewBag.OrganizationRoles = organizationRoles;
                        return View(usersInOrganization);
                    }
                }
                
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrganization(int organizationId)
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
                HttpContext.Session.SetInt32("OrganizationId", organizationId);
                HttpContext.Session.SetString("UserRole", userRole.Role);
                HttpContext.Session.SetString("OrganizationName", userRole.Organization.Name);

                // Set the organization ID and user role in ViewBag to access them in the view
                ViewBag.OrganizationId = organizationId;
                ViewBag.UserRole = userRole.Role;
                ViewBag.OrganizationName = userRole.Organization.Name;
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
