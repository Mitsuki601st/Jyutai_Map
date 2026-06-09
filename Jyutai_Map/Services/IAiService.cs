namespace Jyutai_Map.Services
{
    public interface IAiService
    {
        Task<string> ChatAsync(string message);
        IAsyncEnumerable<string> StreamChatAsync(string message);
    }
}
