using PW.Models;

namespace PW.Models.ViewModels
{
    public class PsychologistDashboardViewModel
    {
        // Statistics
        public int TotalStudents { get; set; }

        public int TodayAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int HighRiskStudents { get; set; }

        // Tables
        public List<Appointment> RecentAppointments { get; set; }
            = new();

        public List<PsychologicalProfile> HighRiskCases { get; set; }
            = new();
    }
}