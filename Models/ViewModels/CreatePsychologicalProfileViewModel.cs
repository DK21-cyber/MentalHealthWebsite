using Microsoft.AspNetCore.Mvc.Rendering;
using PW.Models;
using System.ComponentModel.DataAnnotations;

namespace PW.Models.ViewModels
{
    public class CreatePsychologicalProfileViewModel
    {
        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ProfileStatus Status { get; set; }

        [Display(Name = "Diagnosis")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> Students { get; set; }
            = new List<SelectListItem>();
    }
}