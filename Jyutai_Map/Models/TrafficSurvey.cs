using System.ComponentModel.DataAnnotations;

namespace Jyutai_Map.Models
{
    public class TrafficSurvey
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "評価は1から5の間で入力してください。")]
        [Display(Name = "五段階評価")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "コメントは300文字以内で入力してください。")]
        [Display(Name = "コメント")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "投稿日時")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
