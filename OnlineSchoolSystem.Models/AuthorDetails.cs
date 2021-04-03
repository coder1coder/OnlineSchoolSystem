using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Models
{
    public class AuthorDetails
    {
        public string ChannelId { get; set; }
        public string ChannelUrl { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsVerified { get; set; }
        public bool IsChatOwner { get; set; }
        public bool IsChatSponsor { get; set; }
        public bool IsChatModerator { get; set; }

    }
}
