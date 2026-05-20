using System.ComponentModel.DataAnnotations;

namespace Jyutai_Map.Models
{
    public class TrafficReport
    {
        public int Id { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public int CongestionLevel { get; set; } // 1 to 5

        public string? Description { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    }
}
