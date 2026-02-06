using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models;
using System.Diagnostics;

namespace SentinelAnalytics.Controllers
{
    public class HomeController(
        UserManager<IdentityUser> userManager,
        SentinelDbContext _db) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermsOfService()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var projects = await _db.Projects.OrderByDescending(p => p.CreatedDate).ToListAsync();
            return View("User", projects);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject(string name, string platform)
        {
            if (string.IsNullOrEmpty(name)) 
                return BadRequest("Project name is required.");

            var user = userManager.GetUserId(User)
                ?? throw new InvalidDataException("User not login");

            var newProject = new Project
            {
                UserId = user,
                Name = name,
                Platform = platform,
                CreatedDate = DateTime.UtcNow
            };

            _db.Projects.Add(newProject);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(UserProfile));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
