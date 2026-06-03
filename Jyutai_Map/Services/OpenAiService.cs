using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

namespace Jyutai_Map.Services
{
    public class OpenAiService : IAiService
    {
        private readonly ChatClient _client;

        public OpenAiService(IConfiguration configuration)
        {
            var apiKey = configuration["ApiKeys:OpenAI"];
            var model = configuration["OpenAI:Model"] ?? "gpt-4o";
            
            // OpenAI クライアントの初期化
            _client = new ChatClient(model, apiKey);
        }

        public async Task<string> ChatAsync(string message)
        {
            try
            {
                ChatCompletion completion = await _client.CompleteChatAsync(message);
                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"OpenAI APIエラー: {ex.Message}";
            }
        }
    }
}
