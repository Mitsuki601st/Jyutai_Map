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
        private readonly string _model;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:Gemini"];
            // appsettings.json からモデル名を取得、未設定なら gemini-1.5-flash をデフォルトにする
            _model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
        }

        public async Task<string> ChatAsync(string message)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY")
                return "Gemini APIキーが正しく読み込まれていないか、プレースホルダーのままです。appsettings.jsonを確認してください。";

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

            int maxRetries = 3;
            int delayMs = 2000;

            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    // リトライごとに新しいStringContentを作成
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(url, content);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && i < maxRetries)
                    {
                        // 429エラーの場合は待機してリトライ
                        await Task.Delay(delayMs);
                        delayMs *= 2; // 指数バックオフ
                        continue;
                    }

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
                    if (i < maxRetries)
                    {
                        await Task.Delay(delayMs);
                        delayMs *= 2;
                        continue;
                    }
                    return $"通信エラーが発生しました: {ex.Message}";
                }
            }
            
            return "リトライ上限に達しました。時間をおいて再度お試しください。";
        }
    }
}
