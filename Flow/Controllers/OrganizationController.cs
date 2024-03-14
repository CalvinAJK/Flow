using Flow.Data;
using Flow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly FlowContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizationController(FlowContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }

        [Authorize]
        // GET: OrganizationController
        public ActionResult Index()
        {
            // Retrieve the user ID of the currently logged-in user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the organizations that the user is in
            var userOrganizations = _context.OrganizationRoles
                .Where(or => or.UserId == userId && or.Organization.IsDeleted == false)
                .Select(or => or.Organization)
                .ToList();
            return View(userOrganizations);
        }

        // GET: OrganizationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
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

                return RedirectToAction("Index", "Home");
            }
            return View(organization);
        }

        // GET: OrganizationController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            return View(organization);
        }

        // POST: OrganizationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Organization organization)
        {
            if (id != organization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(organization.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
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


        // GET: OrganizationRoles/Kick/{id}
        public async Task<IActionResult> Kick(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationRole = await _context.OrganizationRoles
                .Include(or => or.Organization) // Assuming Organization is a navigation property
                .FirstOrDefaultAsync(m => m.Id == id);

            if (organizationRole == null)
            {
                return NotFound();
            }
            // Retrieve the user based on UserId from the organization role
            var user = await _userManager.FindByIdAsync(organizationRole.UserId.ToString());
            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound("User not found.");
            }
            ViewBag.UserName = user.UserName;
            return View(organizationRole);
        }

        // POST: OrganizationRoles/Kick/{id}
        [HttpPost, ActionName("Kick")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KickConfirmed(int id)
        {
            var organizationRole = await _context.OrganizationRoles.FindAsync(id);
            if (organizationRole != null)
            {
                _context.OrganizationRoles.Remove(organizationRole);
                await _context.SaveChangesAsync();
            }

            // Redirect to Home/Index
            return RedirectToAction("Index", "Home");
        }


    }
}
