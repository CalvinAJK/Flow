using Flow.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class ProjectController : Controller
    {
        private readonly FlowContext _context;
        private const string SessionTeamId = "_TeamId";

        public ProjectController(FlowContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: ProjectController
        public async Task<IActionResult> Index()
        {
            // Retrieve the user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user has an existing team selection in the session
            var teamId = HttpContext.Session.GetInt32(SessionTeamId);

            // If the user doesn't have an existing team selection, select the first organization from the database
            if (teamId == null)
            {
                // Find the first organization that the user belongs to based on the OrganizationRole table
                var firstTeam = await _context.TeamRoles
                    .Where(or => or.UserId == userId)
                    .Select(or => or.Team)
                    .FirstOrDefaultAsync();

                if (firstTeam != null)
                {
                    HttpContext.Session.SetInt32(SessionTeamId, firstTeam.Id);
                    SetUserTeamRole(userId, firstTeam.Id);

                    var projectInTeam = await _context.Projects
                            .Where(p => p.TeamId == firstTeam.Id)
                            .ToListAsync();

                    return View(projectInTeam);
                }
            }
            else if (teamId != null)
            {
                var selectedTeam = _context.Teams.FirstOrDefault(or => or.Id == teamId);
                if (selectedTeam != null)
                {
                    // Retrieve the user's role based on the organization
                    SetUserTeamRole(userId, teamId.Value);

                    var projectInTeam = await _context.Projects
                            .Where(p => p.TeamId == teamId)
                            .ToListAsync();

                    // Retrieve the team role
                    var projectRole = await _context.TeamRoles
                        .Where(or => or.TeamId == teamId && or.UserId == userId)
                        .ToListAsync();

                    ViewBag.ProjectRoles = projectRole;
                    return View(projectInTeam);
                }
            }

            return View();
        }

        // GET: ProjectController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProjectController/Create
        public ActionResult Create()
        {
            var userTeamId = HttpContext.Session.GetInt32("TeamId");
            if (userTeamId == null)
            {
                TempData["ErrorMessage"] = "You must be in an team first.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [Authorize]
        // POST: ProjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                var userTeamId = HttpContext.Session.GetInt32("TeamId");
                project.TeamId = userTeamId.Value;
                _context.Add(project);
                await _context.SaveChangesAsync();

                // Automatically assign the user who created the project as manager
                ProjectRole projectRole = new ProjectRole
                {
                    ProjectId = project.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Role = "Manager"
                };
                _context.ProjectRoles.Add(projectRole);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: ProjectController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectController/Edit/5
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

        // GET: ProjectController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectController/Delete/5
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

        [HttpPost]
        public async Task<IActionResult> ChangeTeam(int teamId)
        {
            // Update the session with the selected team ID
            HttpContext.Session.SetInt32(SessionTeamId, teamId);

            // Retrieve user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Set the user's role based on the selected organization
            SetUserTeamRole(userId, teamId);

            // Return JSON indicating success and redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("Index", "Project") });
        }

        private void SetUserTeamRole(string userId, int teamId)
        {
            var userRole = _context.TeamRoles
                .Include(or => or.Team)
                .FirstOrDefault(or => or.UserId == userId && or.TeamId == teamId);


            if (userRole != null)
            {
                // Set the user's role in the session or claims
                HttpContext.Session.SetInt32("TeamId", teamId);
                HttpContext.Session.SetString("UserTeamRole", userRole.Role);
                HttpContext.Session.SetString("TeamName", userRole.Team.Name);

                // Set the team ID and user role in ViewBag to access them in the view
                ViewBag.TeamId = teamId;
                ViewBag.UserTeamRole = userRole.Role;
                ViewBag.TeamName = userRole.Team.Name;
            }
        }
    }
}
