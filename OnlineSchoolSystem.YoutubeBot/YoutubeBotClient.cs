using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineSchoolSystem.YoutubeBot
{
    public class YoutubeBotClient
    {
        private HttpClient _client;

        public string Token = string.Empty;

        public YoutubeBotClient(string token)
        {
            Token = token;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("referer", "www.example.com:8000/*");
            _client.DefaultRequestHeaders.Add("accept", "applications/json");
        }

        /// <summary>
        /// Получает сообщения чата
        /// </summary>
        /// <param name="part">id, snippet, and authorDetails</param>
        /// <returns></returns>
        public IEnumerable<Message> GetLiveChatMessages(string liveChatId)
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "liveChatId", liveChatId },
                { "part", "snippet" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("https://www.googleapis.com/youtube/v3/liveChat/messages?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jobj = JObject.Parse(result);

                if (jobj.ContainsKey("items") && jobj["items"].Type == JTokenType.Array && jobj["items"].HasValues)
                {
                    foreach (var item in jobj["items"])
                    {
                        yield return new Message()
                        {
                            Id = item.Value<string>("id"),
                            TextMessage = item["snippet"]["textMessageDetails"].Value<string>("messageText"),
                        };
                    }
                }
                    
            }
            else throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Получение идентификатора чата по идентификатору трансляции
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        public string GetLiveChatIdByBroadcastId(string broadcastId)
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "id", broadcastId },
                { "part", "snippet" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("https://www.googleapis.com/youtube/v3/liveBroadcasts?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jObj = JObject.Parse(result);

                if (jObj.ContainsKey("items") && jObj["items"].Type == JTokenType.Array && jObj["items"].HasValues)
                    return jObj["items"][0]["snippet"].Value<string>("liveChatId");
                else 
                    throw new Exception("Bad response");
            }
            else throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Получение всех трансляций
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        public IEnumerable<Broadcast> GetBroadcasts()
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    { "part", "id,snippet" },
                    { "mine", "true" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("https://www.googleapis.com/youtube/v3/liveBroadcasts?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jObj = JObject.Parse(result);

                if (jObj.ContainsKey("items") && jObj["items"].Type == JTokenType.Array && jObj["items"].HasValues)
                {
                    foreach (var item in jObj["items"].Children())
                        yield return new Broadcast()
                        {
                            Id = item.Value<string>("id"),
                            Title = item["snippet"].Value<string>("title"),
                            LiveChatId = item["snippet"].Value<string>("liveChatId"),
                        };
                }
                else
                    throw new Exception("Bad response");
            }
            else throw new Exception(response.ReasonPhrase);
        }
    }
}
