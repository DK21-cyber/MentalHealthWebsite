namespace PW.Models.ViewModels
{
    public class AppointmentDetailViewModel
    {
        public int Id { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string PsychologistName { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Notes { get; set; }

        public DateTime AppointmentDate { get; set; }

        public DateTime Time { get; set; }

        public string? Description { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}