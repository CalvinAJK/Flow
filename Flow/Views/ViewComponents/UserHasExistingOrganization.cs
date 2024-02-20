using Flow.Data;
using Microsoft.AspNetCore.Mvc;

namespace Flow.Views.ViewComponents
{
    public class UserHasExistingOrganization : ViewComponent
    {
        private readonly FlowContext _context;

        public UserHasExistingOrganization(FlowContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            bool userHasExistingOrganization = _context.OrganizationRoles.Any(o => o.UserId == userId);

            // If you need to retrieve user's organizations, you can do so here
            var userOrganizations = _context.OrganizationRoles
                .Where(o => o.UserId == userId)
                .Select(o => o.Organization)
                .ToList();

            return View(userHasExistingOrganization ? "UserHasExistingOrganization" : "UserHasNoExistingOrganization", userOrganizations);
        }
    }
}
