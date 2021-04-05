using System.Collections.Generic;

namespace OnlineSchoolSystem.Models.Configurations
{
    public class YoutubeBotConfig
	{
		public Dictionary<string, string> Endpoints { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Token { get; set; }
		public int Interval { get; set; }
    }
}
