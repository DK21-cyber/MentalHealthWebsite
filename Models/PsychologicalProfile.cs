namespace PW.Models
{
    public class PsychologicalProfile
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int PsychologistId { get; set; }

        public ProfileStatus Status { get; set; }
            = ProfileStatus.Normal;

        public string? Diagnosis { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        public Student? Student { get; set; }

        public Psychologist? Psychologist { get; set; }

        
    }

    public enum ProfileStatus
    {
        Normal,
        Monitoring,
        Counseling,
        Risking
    }
}