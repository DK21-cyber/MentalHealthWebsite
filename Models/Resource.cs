namespace PW.Models
{
    public class Resource
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Type { get; set; }
        // PDF
        // Video
        // Website
        // Audio

        public int CategoryId { get; set; }

        public ResourceCategory? Category { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
