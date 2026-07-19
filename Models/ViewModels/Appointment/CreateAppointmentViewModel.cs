using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PW.Models.ViewModels
{
    public class CreateAppointmentViewModel
    {
        [Required]
        [Display(Name = "Psychologist")]
        public int PsychologistId { get; set; }

        [Required]
        [Display(Name = "Available Time Slot")]
        public int ScheduleSlotId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        public IEnumerable<SelectListItem>? Psychologists { get; set; }

        public IEnumerable<SelectListItem>? ScheduleSlots { get; set; }
    }
}