using Flow.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flow.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {

        private readonly FlowContext _context;
        private string _userId;

        public OrganizationController(FlowContext context)
        {
            _context = context;
            _userId = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "NameIdentifier")?.Value;

        }


        /*        // GET User's Organizations
                public IActionResult GetUserOrganizations()
                {
                    string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                    bool userHasExistingOrganization = _context.OrganizationRoles.Any(o => o.UserId == userId);

                    // If you need to retrieve user's organizations, you can do so here
                    var userOrganizations = _context.OrganizationRoles
                        .Where(o => o.UserId == userId)
                        .Select(o => o.Organization)
                        .ToList();

                    // Create a ViewModel to pass data to the layout
                    var viewModel = new OrganizationLayoutViewModel
                    {
                        UserHasExistingOrganization = userHasExistingOrganization,
                        UserOrganizations = userOrganizations
                    };

                    return View(viewModel);
                }
        */
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Organization organization)
        {

                _context.Add(organization);
                await _context.SaveChangesAsync();

            // Automatically assign the user who created the organization as admin
            OrganizationRole orgRole = new OrganizationRole
                {
                    OrganizationId = organization.Id,
                    UserId = _userId,
                    Role = "Admin"
                };
                _context.OrganizationRoles.Add(orgRole);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), "Home");
            
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
