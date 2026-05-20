using Jyutai_Map.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Jyutai_Map.Controllers
{
    public class HomeController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public HomeController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var reports = _context.TrafficReports.OrderByDescending(r => r.ReportedAt).ToList();
            return View(reports);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
