namespace PW.Models
{
    public class ReadingHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BlogId { get; set; }

        public DateTime ViewedAt { get; set; }

        public ApplicationUser? User { get; set; }

        public Blog? Blog { get; set; }
    }
}
