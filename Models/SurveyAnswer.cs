namespace PW.Models
{
    public class SurveyAnswer
    {
        public int Id { get; set; }

        public int SurveyResultId { get; set; }

        public int QuestionId { get; set; }

        public int QuestionOptionId { get; set; }

        public SurveyResult? SurveyResult { get; set; }

        public Question? Question { get; set; }

        public QuestionOption? QuestionOption { get; set; }
    }
}