using Flow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flow.Views.ViewComponents
{
    public class UserHasExistingTeam : ViewComponent
    {
        private readonly FlowContext _context;

        public UserHasExistingTeam(FlowContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var userOrgId = HttpContext.Session.GetInt32("OrganizationId");

            // Check if the user has existing teams associated with the organization
            bool userHasExistingTeam = await _context.TeamRoles
                .AnyAsync(tr => tr.UserId == userId &&
                                tr.Team.OrganizationId == userOrgId &&
                                !tr.Team.IsDeleted);

            // If you need to retrieve user's teams, you can do so here
            var userTeams = await _context.TeamRoles
                .Where(tr => tr.UserId == userId &&
                             tr.Team.OrganizationId == userOrgId &&
                             !tr.Team.IsDeleted)
                .Select(tr => tr.Team)
                .ToListAsync();

            // Retrieve organization ID from session
            int? teamId = HttpContext.Session.GetInt32("TeamId");
            ViewBag.TeamId = teamId; // Pass organization ID to the ViewBag

            return View(userHasExistingTeam ? "UserHasExistingTeam" : "UserHasNoExistingTeam", userTeams);
        }
    }
}
