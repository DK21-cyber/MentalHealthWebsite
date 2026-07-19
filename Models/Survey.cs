using System.ComponentModel.DataAnnotations;

namespace PW.Models
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }

        public int SurveyCategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Người tạo khảo sát
        public int? PsychologistId { get; set; }

        public Psychologist? Psychologist { get; set; }

        public SurveyCategory SurveyCategory { get; set; }

        public ICollection<Question> Questions { get; set; }
            = new List<Question>();

        public ICollection<SurveyResult> Results { get; set; }
            = new List<SurveyResult>();
    }
}