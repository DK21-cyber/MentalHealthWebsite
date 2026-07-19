using System.ComponentModel.DataAnnotations;

namespace PW.Models.ViewModels
{
    public class StudentProfileViewModel
    {
        public StudentProfileViewModel() { }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Student Code")]
        public string StudentCode { get; set; } = string.Empty;

        [Display(Name = "Class")]
        public string Class { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public ProfileStatus Status { get; set; }
            = ProfileStatus.Normal;

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public enum ProfileStatus
        {
            Normal,
            Monitoring,
            Counseling,
            Risking
        }
    }
}