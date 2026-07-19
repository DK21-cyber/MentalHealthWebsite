namespace PW.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string? Thumbnail { get; set; }

        public int CategoryId { get; set; }

        public BlogCategory? Category { get; set; }

        public string AuthorId { get; set; }

        public ApplicationUser? Author { get; set; }

        public bool IsPublished { get; set; }

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comment> Comments { get; set; }
            = new List<Comment>();
    }
}
