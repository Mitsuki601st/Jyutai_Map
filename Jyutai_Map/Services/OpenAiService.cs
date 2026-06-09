using OpenAI.Chat;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

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
                List<ChatMessage> messages = new List<ChatMessage>
                {
                    new SystemChatMessage("あなたはホーチミン市1区（District 1, Ho Chi Minh City）に特化したアシスタントです。おすすめの場所や施設、情報について聞かれた際は、必ずホーチミン市1区内のものに限定して回答してください。また、利用者が他のエリアについて尋ねた場合も、1区内の関連情報を提供するように努めてください。"),
                    new UserChatMessage(message)
                };
                ChatCompletion completion = await _client.CompleteChatAsync(messages);
                return completion.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"OpenAI APIエラー: {ex.Message}";
            }
        }

        public async IAsyncEnumerable<string> StreamChatAsync(string message)
        {
            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage("あなたはホーチミン市1区（District 1, Ho Chi Minh City）に特化したアシスタントです。おすすめの場所や施設、情報について聞かれた際は、必ずホーチミン市1区内のものに限定して回答してください。また、利用者が他のエリアについて尋ねた場合も、1区内の関連情報を提供するように努めてください。"),
                new UserChatMessage(message)
            };

            AsyncCollectionResult<StreamingChatCompletionUpdate> updates = _client.CompleteChatStreamingAsync(messages);

            await foreach (StreamingChatCompletionUpdate update in updates)
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(updatePart.Text))
                    {
                        yield return updatePart.Text;
                    }
                }
            }
        }
    }
}
