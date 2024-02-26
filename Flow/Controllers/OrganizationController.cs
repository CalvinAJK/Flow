using Flow.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }

        // GET: OrganizationController
        public ActionResult Index()
        {
            var organizations = _context.Organizations.Where(o => o.IsDeleted == false).ToList();
            return View(organizations);
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
    }
}
