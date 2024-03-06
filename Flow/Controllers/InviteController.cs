using Flow.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // GET: InviteController
        public ActionResult Index()
        {
            // Retrieve the user ID of the currently logged-in user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve invitations where userId matches either InviterId or InvitedId
            var invites = _context.Invitations
                .Where(i => i.InviterId == userId || i.InvitedId == userId)
                .OrderByDescending(i => i.DateCreated)
                .ToList();
            return View(invites);
        }

        // GET: InviteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InviteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InviteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
    }
}
