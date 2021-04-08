using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineSchoolSystem.Bots.Models;
using OnlineSchoolSystem.Bots.Youtube.Models;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.Models.Configurations;
using OnlineSchoolSystem.Utilites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineSchoolSystem.Bots
{
    public class YoutubeBot: IBot
    {
        private readonly YoutubeBotConfig _config;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Task _task;
        private readonly IStorage _storage;
        private HttpClient _client;

        public TaskStatus Status => _task.Status;

        public YoutubeBot(YoutubeBotConfig config, IStorage storage)
        {
            _config = config;
            _storage = storage;
            _tokenSource = new CancellationTokenSource();

            _task = new Task(() => DoWork(), _tokenSource.Token);
        }

        public bool CanStart()
        {
            return
                string.IsNullOrWhiteSpace(_config.ClientId) == false
                &&
                string.IsNullOrWhiteSpace(_config.ClientSecret) == false
                &&
                (_task.Status == TaskStatus.Canceled || _task.Status == TaskStatus.Created);
        }

        public bool Start()
        {
            if (CanStart())
            {
                DoOAuthAsync(_config.ClientId, _config.ClientSecret)
                        .Wait();

                if (string.IsNullOrWhiteSpace(_config.Token) == false)
                {
                    _task.Start();
                    return true;
                }
            }

            return false;
        }

        private void DoWork()
        {
            Helper.Log("Погодотвка к работе"); 
            _client = new HttpClient
            {
                BaseAddress = new Uri(_config.Endpoints["request"])
            };

            _client.DefaultRequestHeaders.Add("referer", "www.example.com:8000/*");
            _client.DefaultRequestHeaders.Add("accept", "applications/json");

            while (_tokenSource.IsCancellationRequested == false)
            {
                Helper.Log("Работаю..");

                //Do check _token's expire

                if (string.IsNullOrWhiteSpace(_config.Token))
                {
                    Helper.Log("Некорректный токен", Helper.LogLevel.Error);
                    _tokenSource.Cancel();
                    return;
                }

                var broadcasts = GetBroadcasts().ToList();

                if (broadcasts.Count > 0)
                {
                    var liveChatId = GetLiveChatIdByBroadcastId(broadcasts.First().Id);

                    var youtubeMessages = GetLiveChatMessages(liveChatId).ToList();

                    var messages = youtubeMessages.Select(x => new Message()
                    {
                        Member = new Member()
                        {
                            Name = x.AuthorDetails.DisplayName
                        },
                        Text = x.Snippet.TextMessageDetails.MessageText
                    });

                    _storage.Store(messages);

                    //AddMessagesToFile(broadcasts.First().Id, messages);

                    //foreach (var item in youtubeMessages)
                    //    Helper.Log(item.ToString(), Helper.LogLevel.Success);

                    //send message if question registered
                    //if (!bot.SendTextMessage(liveChatId, "messages count: " + messages.Count))
                    //{
                    //    Helper.Log("Отправка сообщения не удалась", Helper.LogLevel.Error);
                    //}
                }

                Helper.Log($"Ожидаю {_config.Interval}ms");
                Thread.Sleep(_config.Interval);
            }
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            //_tokenSource.Dispose();
        }

        /// <summary>
        /// Получает сообщения чата
        /// </summary>
        /// <param name="part">id, snippet, and authorDetails</param>
        /// <returns></returns>
        private IEnumerable<YoutubeMessage> GetLiveChatMessages(string liveChatId)
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "liveChatId", liveChatId },
                { "part", "id,snippet,authorDetails" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _config.Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("liveChat/messages?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jobj = JObject.Parse(result);

                if (jobj.ContainsKey("items") && jobj["items"].Type == JTokenType.Array && jobj["items"].HasValues)
                {
                    return
                        JsonConvert.DeserializeObject<List<YoutubeMessage>>(jobj["items"].ToString())
                        ??
                        Enumerable.Empty<YoutubeMessage>();
                }
                else return Enumerable.Empty<YoutubeMessage>();

            }
            else throw new Exception(response.ReasonPhrase);
        }

        /// <summary>
        /// Получение идентификатора чата по идентификатору трансляции
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        private string GetLiveChatIdByBroadcastId(string broadcastId)
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "id", broadcastId },
                { "part", "snippet" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _config.Token));

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
        private IEnumerable<Broadcast> GetBroadcasts()
        {
            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    { "part", "id,snippet" },
                    { "broadcastStatus", "active" },
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _config.Token));

            var parameters = encodedContent.ReadAsStringAsync().Result;

            var response = _client.GetAsync("liveBroadcasts?" + parameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                var jObj = JObject.Parse(result);

                if (jObj.ContainsKey("items") && jObj["items"].Type == JTokenType.Array)
                {
                    if (jObj["items"].HasValues)
                    {
                        foreach (var item in jObj["items"].Children())
                            yield return new Broadcast()
                            {
                                Id = item.Value<string>("id"),
                                Title = item["snippet"].Value<string>("title"),
                                LiveChatId = item["snippet"].Value<string>("liveChatId"),
                            };
                    }                        
                }
                else Helper.Log("Некорректный ответ от сервера", Helper.LogLevel.Error);
            }
            else Helper.Log("Некорректный ответ от сервера", Helper.LogLevel.Error);
        }

        /// <summary>
        /// Отправляет текстовое сообщение в указанный чат
        /// </summary>
        /// <param name="liveChatId">ID чата</param>
        /// <param name="message">Максимальная длина сообщения огранчена в 200 символов, сообщения свыше обрезаются</param>
        private bool SendTextMessage(string liveChatId, string message)
        {
            if (message.Length > 200)
                message = message.Substring(0, 200);

            var encodedContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                { "part", "snippet" },
                { "alt", "json" }
            });

            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _config.Token));

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

        private async Task DoOAuthAsync(string clientId, string clientSecret)
        {
            Helper.Log($"Попытка авторизации");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                Helper.Log($"Неверные значение ClientId и ClientSecret", Helper.LogLevel.Error);
                return;
            }

            var state = GenerateRandomDataBase64url(32);
            var codeVerifier = GenerateRandomDataBase64url(32);
            var codeChallenge = Base64UrlEncodeNoPadding(Sha256Ascii(codeVerifier));
            const string codeChallengeMethod = "S256";

            var redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();

            string[] scopes =
            {
                "https://www.googleapis.com/auth/youtube.readonly",
                "https://www.googleapis.com/auth/youtube.force-ssl"
            };

            var scopesString = string.Join(' ', scopes);

            var endpoint = _config.Endpoints["auth"];

            var authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
                endpoint,
                scopesString,
                Uri.EscapeDataString(redirectUri),
                clientId,
                state,
                codeChallenge,
                codeChallengeMethod);

            var targetURL = authorizationRequest;
            var psi = new ProcessStartInfo
            {
                FileName = targetURL,
                UseShellExecute = true
            };

            Process.Start(psi);

            var context = await http.GetContextAsync();

            var response = context.Response;
            string responseString = "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
            http.Stop();

            string error = context.Request.QueryString.Get("error");

            if (error is object)
            {
                Helper.Log($"Ошибка авторизации OAuth: {error}.", Helper.LogLevel.Error);
                return;
            }
            if (context.Request.QueryString.Get("code") is null
                || context.Request.QueryString.Get("state") is null)
            {
                Helper.Log($"Некорректный ответ после авторизации. {context.Request.QueryString}");
                return;
            }

            var code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            if (incomingState != state)
            {
                Helper.Log($"Полученный ответ содержит неверное состояние ({incomingState})");
                return;
            }

            await ExchangeCodeFor_tokensAsync(code, codeVerifier, redirectUri, clientId, clientSecret);
        }

        /// <summary>
        /// Меняет полученный код при авторизации на токен
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeVerifier"></param>
        /// <param name="redirectUri"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        private async Task ExchangeCodeFor_tokensAsync(string code, string codeVerifier, string redirectUri, string clientId, string clientSecret)
        {
            var _tokenRequestUri = "https://www.googleapis.com/oauth2/v4/token";
            var _tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectUri),
                clientId,
                codeVerifier,
                clientSecret
                );

            var _tokenRequest = (HttpWebRequest)WebRequest.Create(_tokenRequestUri);
            _tokenRequest.Method = "POST";
            _tokenRequest.ContentType = "application/x-www-form-urlencoded";
            _tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var _tokenRequestBodyBytes = Encoding.ASCII.GetBytes(_tokenRequestBody);
            _tokenRequest.ContentLength = _tokenRequestBodyBytes.Length;
            using (Stream requestStream = _tokenRequest.GetRequestStream())
            {
                await requestStream.WriteAsync(_tokenRequestBodyBytes, 0, _tokenRequestBodyBytes.Length);
            }

            try
            {
                var _tokenResponse = await _tokenRequest.GetResponseAsync();
                using (var reader = new StreamReader(_tokenResponse.GetResponseStream()))
                {
                    var responseText = await reader.ReadToEndAsync();

                    var _tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    _config.Token = _tokenEndpointDecoded["access_token"];
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseText = await reader.ReadToEndAsync();
                            Helper.Log("HTTP: " + response.StatusCode, Helper.LogLevel.Error);
                            Helper.Log(responseText, Helper.LogLevel.Error);
                        }
                    }
                }
            }
        }

        // ref http://stackoverflow.com/a/3978040
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private static string GenerateRandomDataBase64url(uint length)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlEncodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string, which is assumed to be ASCII.
        /// </summary>
        private static byte[] Sha256Ascii(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(bytes);
            }
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string Base64UrlEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
