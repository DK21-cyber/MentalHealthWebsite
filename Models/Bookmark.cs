namespace PW.Models
{
    public class Bookmark
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BlogId { get; set; }

        public DateTime SavedAt { get; set; }

        public Blog? Blog { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
