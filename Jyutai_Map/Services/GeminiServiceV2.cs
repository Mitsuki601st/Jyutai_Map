using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Jyutai_Map.Services
{
    public class GeminiServiceV2 : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string _model;

        public GeminiServiceV2(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Gemini"];
            _model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
        }

        public async Task<string> ChatAsync(string message)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY")
                return "Gemini APIキーが正しく設定されていません。appsettings.jsonを確認してください。";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
            
            var payload = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = "あなたはホーチミン市1区（District 1, Ho Chi Minh City）に特化したアシスタントです。おすすめの場所や施設、情報について聞かれた際は、必ずホーチミン市1区内のものに限定して回答してください。また、利用者が他のエリアについて尋ねた場合も、1区内の関連情報を提供するように努めてください。" } }
                },
                contents = new[]
                {
                    new { parts = new[] { new { text = message } } }
                }
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Gemini APIエラー (Status: {response.StatusCode}): {errorContent}";
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
                
                return "有効な回答が得られませんでした。";
            }
            catch (Exception ex)
            {
                return $"通信エラーが発生しました: {ex.Message}";
            }
        }
    }
}
