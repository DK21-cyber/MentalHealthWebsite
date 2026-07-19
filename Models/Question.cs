using System.ComponentModel.DataAnnotations;

namespace PW.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public int OrderNumber { get; set; }

        public Survey? Survey { get; set; }

        public ICollection<QuestionOption> Options { get; set; }
            = new List<QuestionOption>();
    }
}