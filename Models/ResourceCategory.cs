namespace PW.Models
{
    public class ResourceCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Resource> Resources { get; set; }
            = new List<Resource>();
    }
}
