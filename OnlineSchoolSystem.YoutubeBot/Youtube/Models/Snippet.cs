using System;

namespace OnlineSchoolSystem.Bots.Youtube.Models
{
    /// <summary>
    /// Модель фрагмента сообщения 
    /// https://developers.google.com/youtube/v3/live/docs/liveChatMessages#snippet.type
    /// </summary>
    public class Snippet
    {
        public enum SnippetType
        {
            chatEndedEvent,
            messageDeletedEvent,
            newSponsorEvent,
            sponsorOnlyModeEndedEvent,
            sponsorOnlyModeStartedEvent,
            superChatEvent,
            superStickerEvent,
            textMessageEvent,
            tombstone
        }
        public SnippetType Type { get; set; }
        public string LiveChatId { get; set; }
        public string AuthorChannelId { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool HasDisplayContentedAt { get; set; }
        public TextMessageDetails TextMessageDetails { get; set; }
        public string DisplayMessage { get; set; }
        public string MessageDeletedDetails { get; set; }
    }
}
