namespace PW.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public StudentDashboardViewModel() { }

        public Student? Student { get; set; }

        public string Name { get; set; } = string.Empty;

        public string StudentCode { get; set; } = string.Empty;

        public string Class { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int TotalSurveys { get; set; }

        public int TotalAppointments { get; set; }

        public int TotalProfiles { get; set; }

        public string? LatestRiskLevel { get; set; }

        public DateTime? LastSurveyDate { get; set; }

        public DateTime? PendingAppointments { get; set; }
    }
}