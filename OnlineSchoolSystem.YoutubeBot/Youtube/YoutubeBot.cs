using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineSchoolSystem.Bots.Models;
using OnlineSchoolSystem.Bots.Youtube.Models;
using OnlineSchoolSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace OnlineSchoolSystem.Bots
{
    public class YoutubeBot: IBot
    {
        private const string _endpoint = "https://www.googleapis.com/youtube/v3/";
        private readonly HttpClient _client;

        public string Token = string.Empty;

        public BotState State { get; set; }

        public YoutubeBot(string token)
        {
            Token = token;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_endpoint)
            };

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
                { "part", "id,snippet,authorDetails" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("liveChat/messages?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jobj = JObject.Parse(result);

                if (jobj.ContainsKey("items") && jobj["items"].Type == JTokenType.Array && jobj["items"].HasValues)
                {
                    return
                        JsonConvert.DeserializeObject<List<Message>>(jobj["items"].ToString())
                        ??
                        Enumerable.Empty<Message>();
                } 
                else return Enumerable.Empty<Message>();

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

            var response = _client.GetAsync("liveBroadcasts?" + parameters).Result;

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
        /// Получение всех активных трансляций
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        public IEnumerable<Broadcast> GetBroadcasts()
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    { "part", "id,snippet" }, 
                    { "broadcastStatus", "active" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("liveBroadcasts?" + parameters).Result;

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
                else throw new Exception("Bad response");
            }
            else throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Отправляет текстовое сообщение в указанный чат
        /// </summary>
        /// <param name="liveChatId">ID чата</param>
        /// <param name="message">Максимальная длина сообщения огранчена в 200 символов, сообщения свыше обрезаются</param>
        public bool SendTextMessage(string liveChatId, string message)
        {
            if (message.Length > 200)
                message = message.Substring(0, 200);

            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "part", "snippet" },
                { "alt", "json" }
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var sendMessageobject = new
            {
                snippet = new
                {
                    liveChatId,
                    type = "textMessageEvent",
                    textMessageDetails = new
                    {
                        messageText = message
                    }
                }
            };

            var sendMessageString = JsonConvert.SerializeObject(sendMessageobject);

            var body = new StringContent(sendMessageString, Encoding.UTF8);

            var response = _client.PostAsync("liveChat/messages?" + parameters, body).Result;

            return response.IsSuccessStatusCode;
        }

        public void Start(IStorage storage)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
