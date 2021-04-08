namespace OnlineSchoolSystem.Models
{
    public interface IAnswer
    {
        public string QuestionId { get; }
        public string Message { get; set; }
    }
}
