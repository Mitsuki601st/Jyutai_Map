using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Jyutai_Map.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:OpenWeatherMap"];
        }

        public async Task<object?> GetWeatherAsync(string city = "Hanoi")
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_OPENWEATHERMAP_API_KEY")
                return new { error = "API Key not configured" };

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=ja";
            try
            {
                return await _httpClient.GetFromJsonAsync<object>(url);
            }
            catch
            {
                return null;
            }
        }
    }
}
