using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAnalytics.Data;
using SentinelAnalytics.Data.Entities;
using SentinelAnalytics.Models;
using SentinelAnalytics.Models.Projects;
using System.Diagnostics;

namespace SentinelAnalytics.Controllers
{
    public class HomeController() : Controller
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
    }
}
