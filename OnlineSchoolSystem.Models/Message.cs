namespace OnlineSchoolSystem.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Member Member { get; set; }
    }
}
