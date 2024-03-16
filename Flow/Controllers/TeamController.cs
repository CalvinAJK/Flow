using Flow.Data;
using Flow.Models;
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

        [Authorize]
        // GET: TeamController
        public ActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userOrgId = HttpContext.Session.GetInt32("OrganizationId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Admin" || userRole == "Moderator")
            {
                var teams = _context.Teams.Where(o => o.OrganizationId == userOrgId && o.IsDeleted == false).ToList();
                ViewBag.UserRole = userRole;
                return View(teams);
            }
            else
            {
                var teams = _context.Teams.Where(o => o.OrganizationId == userOrgId && o.TeamRoles.Any(tr => tr.UserId == userId) && o.IsDeleted == false).ToList();
                ViewBag.UserRole = userRole;
                return View(teams);
            }
            
            
        }

        // GET: TeamController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            var teamUsersRoles = await (from tr in _context.TeamRoles
                                        join u in _context.Users on tr.UserId equals u.Id
                                        where tr.TeamId == id
                                        select new UserTeamRoleViewModel
                                        {
                                            Username = u.UserName, // Adjust this based on your User model
                                            Role = tr.Role
                                        }).ToListAsync();

            ViewData["TeamName"] = team.Name;
            ViewData["TeamDetails"] = teamUsersRoles; // Pass the user-role details to the view
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

        [Authorize]
        // POST: TeamController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,OrganizationId")] Team team)
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
        public async Task<ActionResult> DeleteAsync(int id)
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

        // POST: TeamController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var team = await _context.Teams.FindAsync(id);
                if (team != null)
                {
                    team.IsDeleted = true; // Mark the team as deleted instead of actually deleting it
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
