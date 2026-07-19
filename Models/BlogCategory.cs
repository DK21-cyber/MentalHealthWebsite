namespace PW.Models
{
    public class BlogCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Blog> Blogs { get; set; }
            = new List<Blog>();
    }
}
