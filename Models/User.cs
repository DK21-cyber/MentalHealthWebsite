using System.ComponentModel.DataAnnotations;

namespace PW.Models
{
    public class ApplicationUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string RoleName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Student? Student { get; set; }

        public Psychologist? Psychologist { get; set; }

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();
    }
}