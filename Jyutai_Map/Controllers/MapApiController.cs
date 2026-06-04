using Jyutai_Map.Data;
using Jyutai_Map.Models;
using Jyutai_Map.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jyutai_Map.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapApiController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly IAiService _aiService;
        private readonly ApplicationDbContext _context;

        public MapApiController(WeatherService weatherService, IAiService aiService, ApplicationDbContext context)
        {
            _weatherService = weatherService;
            _aiService = aiService;
            _context = context;
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeather(string city = "Hanoi")
        {
            var data = await _weatherService.GetWeatherAsync(city);
            return Ok(data);
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("メッセージが空です。");

            var response = await _aiService.ChatAsync(request.Message);
            return Ok(new { response });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(string type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _context.SearchHistories
                .Where(h => h.Type == type && h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .Take(10)
                .ToListAsync();
            return Ok(history);
        }

        [HttpPost("history")]
        public async Task<IActionResult> SaveHistory([FromBody] HistoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Type) || string.IsNullOrEmpty(request.Query))
                return BadRequest("Type or Query is missing.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Avoid duplicate recent entries for the same user
            var existing = await _context.SearchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefaultAsync(h => h.Type == request.Type && h.Query == request.Query);

            if (existing != null && (DateTime.UtcNow - existing.CreatedAt).TotalMinutes < 5)
                return Ok();

            var history = new SearchHistory
            {
                UserId = userId,
                Type = request.Type,
                Query = request.Query,
                CreatedAt = DateTime.UtcNow
            };

            _context.SearchHistories.Add(history);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }

    public class HistoryRequest
    {
        public string? Type { get; set; }
        public string? Query { get; set; }
    }
}
