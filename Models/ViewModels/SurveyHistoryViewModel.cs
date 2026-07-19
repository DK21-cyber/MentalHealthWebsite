namespace PW.Models.ViewModels
{
    public class SurveyHistoryViewModel
    {
        public int Id { get; set; }

        public string SurveyName { get; set; } = string.Empty;

        public int TotalScore { get; set; }

        public string RiskLevel { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
    }
}