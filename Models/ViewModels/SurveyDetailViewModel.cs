namespace PW.Models.ViewModels
{
    public class SurveyDetailViewModel
    {
        public int Id { get; set; }

        public string SurveyName { get; set; } = string.Empty;

        public int TotalScore { get; set; }

        public string RiskLevel { get; set; } = string.Empty;

        public string? ResultSummary { get; set; }

        public DateTime SubmittedAt { get; set; }

        public List<SurveyAnswerViewModel> Answers { get; set; }
            = new();
    }

    public class SurveyAnswerViewModel
    {
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int Score { get; set; }
    }
}