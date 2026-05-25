using Jyutai_Map.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jyutai_Map.Controllers
{
    public class SurveyController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public SurveyController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Survey
        public async Task<IActionResult> Index()
        {
            var surveys = await _context.TrafficSurveys.OrderByDescending(s => s.CreatedAt).ToListAsync();
            return View(surveys);
        }

        // GET: Survey/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rating,Comment")] TrafficSurvey survey)
        {
            if (ModelState.IsValid)
            {
                survey.CreatedAt = DateTime.UtcNow;
                _context.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }

        // GET: Survey/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.TrafficSurveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Survey/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rating,Comment,CreatedAt")] TrafficSurvey survey)
        {
            if (id != survey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (survey.CreatedAt.Kind == DateTimeKind.Unspecified)
                    {
                        survey.CreatedAt = DateTime.SpecifyKind(survey.CreatedAt, DateTimeKind.Utc);
                    }
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
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
            return View(survey);
        }

        // GET: Survey/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.TrafficSurveys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // POST: Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = await _context.TrafficSurveys.FindAsync(id);
            if (survey != null)
            {
                _context.TrafficSurveys.Remove(survey);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return _context.TrafficSurveys.Any(e => e.Id == id);
        }
    }
}
