using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{/// <summary>
/// Модель сообщения
/// https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type
/// </summary>
    public class Message : IEquatable<Message>
    {
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public AuthorDetails AuthorDetails { get; set; }

        public bool Equals(Message other)
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
