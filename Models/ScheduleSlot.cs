namespace PW.Models
{
    public class ScheduleSlot
    {
        public int Id { get; set; }

        public int PsychologistId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public SlotStatus Status { get; set; }
            = SlotStatus.Available;

        public string? Notes { get; set; }

        public Psychologist? Psychologist { get; set; }

        public Appointment? Appointment { get; set; }
    }

    public enum SlotStatus
    {
        Available,
        Booked,
        Completed,
        Cancelled
    }
}