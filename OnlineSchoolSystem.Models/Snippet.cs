using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    public class Snippet
    {
        public enum Type
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
        public string LiveChatId { get; set; }
        public string AuthorChannelId { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool HasDisplayContentedAt { get; set; }
        public TextMessageDetails TextMessageDetails { get; set; }
        public string DisplayMessage { get; set; }
        public string MessageDeletedDetails { get; set; }
    }
}
