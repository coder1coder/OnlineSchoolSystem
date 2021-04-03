using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{/// <summary>
/// Модель сообщения
/// https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type
/// </summary>
    public class Message
    {
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public AuthorDetails AuthorDetails { get; set; }

    }
}
