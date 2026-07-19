using PW.Models;
using System.ComponentModel.DataAnnotations;

namespace PW.Models
{
    public class Psychologist
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Degree { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ApplicationUser? User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        public ICollection<ScheduleSlot> ScheduleSlots { get; set; }
            = new List<ScheduleSlot>();

        public ICollection<Survey> Surveys { get; set; }
            = new List<Survey>();

        public ICollection<PsychologicalProfile> PsychologicalProfiles { get; set; }
            = new List<PsychologicalProfile>();
    }
}