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
    public class InviteController : Controller
    {
        private readonly FlowContext _context;

        public InviteController(FlowContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: InviteController
        public ActionResult Index()
        {
            // Retrieve the user ID of the currently logged-in user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = HttpContext.Session.GetString("UserRole");

            // Retrieve invitations where userId matches either InviterId or InvitedId
            var invites = _context.Invitations
                .Where(i => i.InviterId == userId || i.InvitedId == userId)
                .OrderByDescending(i => i.DateCreated)
                .ToList();

            // Enrich invitation data with organization names, inviter emails, invited emails, and formatted date
            var enrichedInvites = invites.Select(invite => new EnrichedInvitation
            {
                Id = invite.Id,
                OrganizationName = _context.Organizations.FirstOrDefault(o => o.Id == invite.OrganizationId)?.Name,
                InviterEmail = _context.Users.FirstOrDefault(u => u.Id == invite.InviterId)?.Email,
                InvitedEmail = _context.Users.FirstOrDefault(u => u.Id == invite.InvitedId)?.Email ?? invite.InvitedId,
                Status = invite.Status,
                DateCreated = invite.DateCreated
            }).ToList();

            ViewBag.UserRole = userRole;
            return View(enrichedInvites);
        }

        // GET: InviteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        // GET: InviteController/Create
        public ActionResult Create()
        {
            // Get the current organization's ID (You need to implement this logic)
            var userOrgId = HttpContext.Session.GetInt32("OrganizationId");
            if (userOrgId == null)
            {
                TempData["ErrorMessage"] = "You must be in an organization first.";
                return RedirectToAction(nameof(Index));
            }

            string? OrgName = HttpContext.Session.GetString("OrganizationName");
            ViewBag.OrganizationName = OrgName;
            return View();
        }

        [Authorize]
        // POST: InviteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                // Get the current organization's ID (You need to implement this logic)
                var currentOrgId = HttpContext.Session.GetInt32("OrganizationId");

                // Get the current user's ID
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Find the user ID based on the email input
                var invitedUserId = await _context.Users
                    .Where(u => u.Email == invitation.InvitedId)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                if (invitedUserId == null)
                {
                    invitedUserId = invitation.InvitedId;
                }


                // Check if the user is already part of the organization
                var isUserAlreadyInOrganization = await _context.OrganizationRoles
                    .AnyAsync(ou => ou.UserId == invitedUserId && ou.OrganizationId == currentOrgId);

                if (isUserAlreadyInOrganization)
                {
                    // Add an error to the model state and return to the view
                    ModelState.AddModelError("", "The user is already part of this organization.");
                    return RedirectToAction("Create");
                }

                // Set the inviter ID and organization ID
                invitation.InviterId = currentUserId;
                invitation.OrganizationId = currentOrgId.Value;

                // Set the invited user's ID
                invitation.InvitedId = invitedUserId;

                // Add the invitation to the context and save changes
                _context.Add(invitation);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(invitation);
        }


        // GET: InviteController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InviteController/Edit/5
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

        // GET: InviteController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InviteController/Delete/5
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

        [Authorize]
        // Revoke Invitation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation != null)
            {
                invitation.Status = "Revoked";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        // Accept Invitation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation != null && invitation.Status == "Pending")
            {
                // Get the current user ID
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Add the current user to the organization specified in the invitation
                var organizationId = invitation.OrganizationId;

                var organizationRole = new OrganizationRole
                {
                    OrganizationId = organizationId,
                    UserId = currentUserId,
                    Role = "Member" // Defaulted to Member role
                };

                _context.OrganizationRoles.Add(organizationRole);

                // Update invitation status to Accepted
                invitation.Status = "Accepted";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
