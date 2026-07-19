namespace PW.Models.ViewModels
{
    public class PsychologicalProfileViewModel
    {
        public int Id { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string PsychologistName { get; set; } = string.Empty;

        public ProfileStatus Status { get; set; }

        public string? Diagnosis { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}