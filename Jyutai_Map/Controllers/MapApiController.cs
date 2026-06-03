using Jyutai_Map.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jyutai_Map.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapApiController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly IAiService _aiService;

        public MapApiController(WeatherService weatherService, IAiService aiService)
        {
            _weatherService = weatherService;
            _aiService = aiService;
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
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }
}
