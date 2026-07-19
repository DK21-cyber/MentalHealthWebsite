using System.ComponentModel.DataAnnotations;

namespace PW.Models.ViewModels
{
    public class EditStudentProfileViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public string StudentCode { get; set; }

        public string Class { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}