using Flow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Policy;

namespace Flow.Views.ViewComponents
{
    public class UserHasChat : ViewComponent
    {
        private readonly FlowContext _context;
        public UserHasChat(FlowContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            const string SessionOrganizationId = "OrganizationId";
            // Check if the user has an existing organization selection in the session
            var organizationId = HttpContext.Session.GetInt32(SessionOrganizationId);
            string organizationName = null; // Default to null if not found

            bool userHasChat = false;

            if (organizationId.HasValue)
            {
                // Query the database to get the organization name
                var organization = await _context.Organizations
                    .Where(o => o.Id == organizationId.Value)
                    .Select(o => new { o.Name }) // Project to anonymous type with only the Name
                    .FirstOrDefaultAsync();
                if (organization != null)
                {
                    organizationName = organization.Name;
                    userHasChat = true; // Assuming having an organization means the user can chat
                }
            }
            else
            {
                userHasChat = false;
            }

            ViewBag.OrganizationName = organizationName;
            // Return the appropriate view based on whether the user has chat capabilities
            return View(userHasChat ? "UserHasChat" : "UserHasNoChat");
        }
            
    }
}
