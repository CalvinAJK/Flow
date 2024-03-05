using Flow.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class TeamController : Controller
    {
        private readonly FlowContext _context;

        public TeamController(FlowContext context)
        {
            _context = context;
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }

        // GET: TeamController
        public ActionResult Index()
        {
            var userOrgId = HttpContext.Session.GetInt32("OrganizationId");

            var teams = _context.Teams.Where(o => o.OrganizationId == userOrgId && o.IsDeleted == false).ToList();
            return View(teams);
        }

        // GET: TeamController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TeamController/Create
        public ActionResult Create()
        {
            var userOrgId = HttpContext.Session.GetInt32("OrganizationId");
            if (userOrgId == null)
            {
                TempData["ErrorMessage"] = "You must be in an organization first.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [Authorize]
        // POST: TeamController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,isDeleted,OrganizationId")] Team team)
        {
            if (ModelState.IsValid)
            {
                var userOrgId = HttpContext.Session.GetInt32("OrganizationId");
                team.OrganizationId = userOrgId.Value;
                _context.Add(team);
                await _context.SaveChangesAsync();

                // Automatically assign the user who created the organization as admin
                TeamRole teamRole = new TeamRole
                {
                    TeamId = team.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Role = "Leader"
                };
                _context.TeamRoles.Add(teamRole);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: TeamController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: TeamController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            return View(team);
        }

        // GET: TeamController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TeamController/Delete/5
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
