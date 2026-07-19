namespace PW.Models
{

    public class BlogRecommendation
    {
        public int Id { get; set; }

        public string ResultLevel { get; set; }

        public int BlogId { get; set; }

        public Blog? Blog { get; set; }
    }
}
