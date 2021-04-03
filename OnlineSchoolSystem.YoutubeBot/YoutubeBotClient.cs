using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OnlineSchoolSystem.YoutubeBot
{
    public class YoutubeBotClient
    {
        private readonly HttpClient _client;
        private readonly string _liveChatId;
        private readonly string _apiKey;

        public YoutubeBotClient(string liveChatId, string apiKey)
        {
            _liveChatId = liveChatId;
            _apiKey = apiKey;

            _client = new HttpClient();

            _client.DefaultRequestHeaders.Add("referer", "www.example.com:8000/*");
            _client.DefaultRequestHeaders.Add("accept", "applications/json");
        }

        /// <summary>
        /// Получает сообщения чата
        /// </summary>
        /// <param name="part">id, snippet, and authorDetails</param>
        /// <returns></returns>
        public IEnumerable<IMessage> GetMessages(string part = "snippet")
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "liveChatId", _liveChatId },
                { "part", part },
                { "key", _apiKey },
            });

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("https://www.googleapis.com/youtube/v3/liveChat/messages?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jobj = JObject.Parse(result);

                if (jobj.ContainsKey("items") && jobj["items"].Type == JTokenType.Array)
                    yield return (IMessage)jobj["items"].Values<IMessage>();
            }
            else throw new Exception(response.ReasonPhrase);
        }
    }
}
