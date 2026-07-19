namespace PW.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int BlogId { get; set; }

        public Blog? Blog { get; set; }

        public string UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;
    }
}
