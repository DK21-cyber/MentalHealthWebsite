namespace PW.Models
{
    public class SurveyResult
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SurveyId { get; set; }

        public int TotalScore { get; set; }

        public string RiskLevel { get; set; }

        public string? ResultSummary { get; set; }

        public string? Stress { get; set; }

        public string? Anxiety { get; set; }

        public string? Depression { get; set; }

        public DateTime SubmittedAt { get; set; }
            = DateTime.Now;

        public Student? Student { get; set; }

        public Survey? Survey { get; set; }

        public ICollection<SurveyAnswer> Answers { get; set; }
            = new List<SurveyAnswer>();
    }
}