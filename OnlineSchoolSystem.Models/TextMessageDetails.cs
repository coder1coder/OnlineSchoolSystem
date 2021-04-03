using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    /// <summary>
    /// Модель деталей сообщения
    /// https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type
    /// </summary>
    public class TextMessageDetails
    {
        public string MessageText { get; set; }
    }
}
