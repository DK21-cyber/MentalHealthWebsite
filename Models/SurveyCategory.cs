namespace PW.Models
{
    public class SurveyCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Survey> Surveys { get; set; }
            = new List<Survey>();
    }
}
