using Flow.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly FlowContext _context;

        public OrganizationController(FlowContext context)
        {
            _context = context;
        }

        // GET User's Organizations
        public IActionResult GetUserOrganizations()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool userHasExistingOrganization = _context.OrganizationRoles.Any(o => o.UserId == userId);

            // If you need to retrieve user's organizations, you can do so here
            var userOrganizations = _context.OrganizationRoles
                .Where(o => o.UserId == userId)
                .Select(o => o.Organization)
                .ToList();

            ViewData["UserHasExistingOrganization"] = userHasExistingOrganization;
            ViewData["UserOrganizations"] = userOrganizations;

            return View();
        }

        // GET: OrganizationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OrganizationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrganizationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrganizationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsDeleted")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                _context.Add(organization);
                await _context.SaveChangesAsync();

                // Automatically assign the user who created the organization as admin
                OrganizationRole orgRole = new OrganizationRole
                {
                    OrganizationId = organization.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Role = "Admin"
                };
                _context.OrganizationRoles.Add(orgRole);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        // GET: OrganizationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrganizationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrganizationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrganizationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
