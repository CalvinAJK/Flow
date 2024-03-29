﻿using Flow.Data;
using Flow.Models;
using Flow.Views.ViewComponents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flow.Controllers
{
    public class ChatController : Controller
    {
        private readonly FlowContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatController(FlowContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: ChatController
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var organizationId = _httpContextAccessor.HttpContext.Session.GetInt32("_OrganizationId");
            string organizationName = null; // Default to null if not found

            if (organizationId == null)
            {
                return RedirectToAction("Index", "Home"); // Redirect if no organization is selected.
               
            }
            // Query the database to get the organization name
            var organization = await _context.Organizations
                .Where(o => o.Id == organizationId.Value)
                .Select(o => new { o.Name }) // Project to anonymous type with only the Name
                .FirstOrDefaultAsync();

            var teams = await _context.TeamRoles
                .Include(tr => tr.Team)
                .Where(tr => tr.UserId == userId && tr.Team.OrganizationId == organizationId)
                .Select(tr => tr.Team)
                .Distinct()
                .ToListAsync();

            var chatViewModel = new ChatViewModel
            {
                OrganizationId = organizationId.Value,
                Teams = teams
            };

            organizationName = organization.Name;
            ViewBag.OrganizationName = organizationName;
            return View(chatViewModel);
        }

        // GET: ChatController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChatController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatController/Create
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

        // GET: ChatController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChatController/Edit/5
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

        // GET: ChatController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChatController/Delete/5
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
