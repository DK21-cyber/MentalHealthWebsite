using PW.Models;

namespace PW.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }

        public int TotalPsychologists { get; set; }

        public int TotalAppointments { get; set; }

        public int HighRiskStudents { get; set; }

        public List<Appointment> RecentAppointments { get; set; }
            = new();

        public List<SurveyResult> RecentSurveyResults { get; set; }
            = new();
    }
}