using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Jyutai_Map.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Gemini"];
        }

        public async Task<string> ChatAsync(string message)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY")
                return "Gemini APIキーが正しく読み込まれていないか、プレースホルダーのままです。appsettings.jsonを確認してください。";

            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}";
            
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = message } } }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            
            try
            {
                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"APIエラーが発生しました (Status: {response.StatusCode}): {errorContent}";
                }
                
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    return candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "回答が空でした。";
                }
                
                return "有効な回答が得られませんでした。APIキーやリクエスト内容を確認してください。";
            }
            catch (Exception ex)
            {
                return $"通信エラーが発生しました: {ex.Message}";
            }
        }
    }
}
