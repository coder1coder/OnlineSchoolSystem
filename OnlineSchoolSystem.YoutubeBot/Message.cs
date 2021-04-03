namespace OnlineSchoolSystem.YoutubeBot
{
    public class Message
    {
        public string Id { get; set; }
        public string TextMessage { get; set; }

        public override string ToString()
        {
            return $"{Id} {TextMessage}";
        }
    }
}