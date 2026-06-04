using System.ComponentModel.DataAnnotations;

namespace Jyutai_Map.Models
{
    public class SearchHistory
    {
        public int Id { get; set; }

        public string? UserId { get; set; } // Nullable for guests

        [Required]
        public string Type { get; set; } = string.Empty; // "Route", "Chat", "Tourist"

        [Required]
        public string Query { get; set; } = string.Empty; // "Origin|Destination" or "Chat message" or "Tourist query"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
