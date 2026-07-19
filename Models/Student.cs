using System.ComponentModel.DataAnnotations;

namespace PW.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        // Liên kết với ASP.NET Identity
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string StudentCode { get; set; }

        [StringLength(50)]
        public string Class { get; set; }

        // ==========================
        // Navigation
        // ==========================

        public ApplicationUser? User { get; set; }

        public ICollection<SurveyResult> SurveyResults { get; set; }
            = new List<SurveyResult>();

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        public ICollection<PsychologicalProfile> PsychologicalProfiles { get; set; }
            = new List<PsychologicalProfile>();

        // Nếu bạn có Comment
        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();
    }
}