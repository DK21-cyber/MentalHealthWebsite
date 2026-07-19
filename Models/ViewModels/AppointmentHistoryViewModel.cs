namespace PW.Models.ViewModels
{
    public class AppointmentHistoryViewModel
    {
        public int Id { get; set; }

        public string PsychologistName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public DateTime Time { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}