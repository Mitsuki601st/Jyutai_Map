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

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Location,CongestionLevel,Description")] TrafficReport report)
        {
            if (ModelState.IsValid)
            {
                report.ReportedAt = DateTime.UtcNow;
                _context.Add(report);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: Home/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = _context.TrafficReports.Find(id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Location,CongestionLevel,Description,ReportedAt")] TrafficReport report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // PostgreSQL requires UTC for 'timestamp with time zone'
                    if (report.ReportedAt.Kind == DateTimeKind.Unspecified)
                    {
                        report.ReportedAt = DateTime.SpecifyKind(report.ReportedAt, DateTimeKind.Utc);
                    }

                    _context.Update(report);
                    _context.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    if (!_context.TrafficReports.Any(e => e.Id == report.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: Home/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = _context.TrafficReports.FirstOrDefault(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var report = _context.TrafficReports.Find(id);
            if (report != null)
            {
                _context.TrafficReports.Remove(report);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
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
