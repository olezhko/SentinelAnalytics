using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Models;
using System.Diagnostics;

namespace SentinelAnalytics.Controllers
{
    public class HomeController(SentinelDbContext db) : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult TermsOfService() => View();
        public IActionResult Support() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Pricing()
        {
            var plans = await db.PricingPlans.OrderBy(p => p.Price).ToListAsync();
            return View(plans);
        }
    }
}
