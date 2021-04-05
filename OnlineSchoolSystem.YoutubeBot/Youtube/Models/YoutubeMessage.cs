using System;

namespace OnlineSchoolSystem.Bots.Youtube.Models
{
    /// <summary>
    /// Модель сообщения
    /// https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type
    /// </summary>
    public class YoutubeMessage : IEquatable<YoutubeMessage>
    {
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public AuthorDetails AuthorDetails { get; set; }

        public bool Equals(YoutubeMessage other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(); 
        }
    }
}
