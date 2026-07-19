namespace PW.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public bool IsPublished { get; set; }

        public int DisplayOrder { get; set; }
    }
}
