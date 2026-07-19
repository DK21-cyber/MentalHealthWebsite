namespace PW.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int PsychologistId { get; set; }

        public int ScheduleSlotId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public DateTime Time { get; set; }

        public string? Description { get; set; }

        public AppointmentStatus Status { get; set; }

        public Student? Student { get; set; }

        public Psychologist? Psychologist { get; set; }

        public ScheduleSlot? ScheduleSlot { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Approved,
        Completed,
        Cancelled
    }
}