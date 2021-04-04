using Newtonsoft.Json;
using OnlineSchoolSystem.DataAccess.FileStorage;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.Utilites;
using OnlineSchoolSystem.Bots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OnlineSchoolSystem.Bots.Youtube.Models;

namespace OnlineSchoolSystem.Client
{
    class Program
    {
        private static JsonSettings _settings;

        static async Task<int> Main(string[] args)
        {
            _settings = new JsonSettings(Environment.CurrentDirectory + "/settings.json");

            var menu = new Menu();
            
            do
            {
                //Инициализация хранилища
                if (!Environment.CurrentDirectory.Contains(_settings.Get("storageDirectoryName")))
                    Directory.CreateDirectory(_settings.Get("storageDirectoryName"));

                menu.PrintMenu();
                var operation = menu.GetSelectedOperation();

                switch (operation)
                {
                    case OperationsEnum.CONNECT_TO_STREAM:

                        var clientId = _settings.Get("clientId");
                        var clientSecret = _settings.Get("clientSecret");

                        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                        {
                            Helper.Log("Не найдены ключи доступа, установите их", Helper.LogLevel.Warning);
                            Console.Write("clientId: ");
                            _settings.Set("clientId", Console.ReadLine());

                            Console.Write("clientSecret: ");
                            _settings.Set("clientSecret", Console.ReadLine());
                            break;
                        }

                        await DoOAuthAsync(clientId, clientSecret);
                        
                        StartBot();

                        break;

                    case OperationsEnum.GET_MESSAGES_FROM_PREVIOUS_STREAMS:
                        {
                            GetMessagesFromPreviousStreams();
                            break;
                        }
                    case OperationsEnum.GET_STATISTIC:
                        {
                            menu.GetUserAnswer("Введите ид стрима для получения статистики");
                            var idStream = Console.ReadLine();
                            GetStatistic(idStream);
                            break;
                        }
                    case OperationsEnum.EXIT:
                        {
                            menu.IsContinue = false;
                            Helper.Log("Нажмите любую клавишу для выхода");
                            Console.ReadKey();
                            return 0;
                        }
                    default:
                        Helper.Log("Некорректная операция", Helper.LogLevel.Error);
                        break;
                }

            } while (menu.IsContinue);

            //exit from app
            return 0;
        }

        /// <summary>
        /// Запускает обработку сообщений ботом
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        private static void StartBot()
        {
            do
            {
                Console.Clear();

                //Do check token's expire

                if (string.IsNullOrWhiteSpace(_settings.Get("Token")))
                {
                    Helper.Log("Некорректный токен", Helper.LogLevel.Error);
                    return;
                }

                Helper.Log("Начинаем работу", Helper.LogLevel.Success);

                var bot = new YoutubeBot(_settings.Get("Token"));

                var broadcasts = bot.GetBroadcasts().ToList();

                if (broadcasts.Count > 0)
                {
                    var liveChatId = bot.GetLiveChatIdByBroadcastId(broadcasts.First().Id);

                    var messages = bot.GetLiveChatMessages(liveChatId).ToList();

                    //save messages to storage
                    AddMessagesToFile(broadcasts.First().Id, messages);

                    foreach (var item in messages)
                        Helper.Log(item.ToString(), Helper.LogLevel.Success);

                    //send message if question registered
                    if (!bot.SendTextMessage(liveChatId, "messages count: " + messages.Count))
                    {
                        Helper.Log("Отправка сообщения не удалась", Helper.LogLevel.Error);
                    }
                }
                else Helper.Log("Нет существующих трансляций", Helper.LogLevel.Error);

                Helper.Log("Я работу свою сделал. Пойду чай попью 5 сек..");
                Thread.Sleep(5000);

                Helper.Log("Press any key to continue working and ESC to exit..");
            }
            while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        /// <summary>
        /// Сохраняет сообщения в хранилище
        /// </summary>
        /// <param name="idStream"></param>
        /// <param name="messages"></param>
        private static void AddMessagesToFile(string idStream, List<Message> messages)
        {
            //проверка на наличие файла в папке, название файла переделать на дату стрима вметсо idStream
            string filePath = Path.Combine(Environment.CurrentDirectory, _settings.Get("storageDirectoryName"), idStream + ".json");

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            var jsonFile = new JsonFileAccess(filePath);
            jsonFile.AppendMessagesToFile(messages);
        }

        /// <summary>
        /// Получает статистику
        /// </summary>
        private static void GetStatistic(string idStream)
        {
            //будет формироваться список авторов с количеством вопросов и ответов для одного стрима             GetReport();
            //будет формироваться список вопросов и ответов для определенного автора по одному стриму. GetAuthorReport(AuthorDetails author) .
            var report = new StatisticPerStream
            {
                LiveChatId = idStream,
                Answers = GetAnswers(idStream),
                Authors = GetAuthors(idStream),
                Questions = GetQuestions(idStream)
            };
        }

        private static List<IAnswer> GetAnswers(string idStream)
        {
            throw new NotImplementedException();
        }

        private static List<AuthorDetails> GetAuthors(string idStream)
        {
            throw new NotImplementedException();
        }

        private static List<IQuestion> GetQuestions(string idStream)
        {
            throw new NotImplementedException();
        }

        private static void GetMessagesFromPreviousStreams()
        {
            string dirname = ""; // получить путь на каталог с файлами по стримам 
            string[] files = Directory.GetFiles(dirname);
            var messages = new List<Message>();
            foreach (var file in files)
            {
                var fileAccess = new JsonFileAccess(file);
                messages.AddRange(fileAccess.ReadAllMessagesFromFile());
            }
            // треубется реализовать получение из списка сообщений только те сообщения, у которых  message type is textMessageEvent.(Linq выражением)
            foreach (var message in messages)
            {
                Helper.Log(message.AuthorDetails.DisplayName + "  " + message.Snippet.DisplayMessage);
            }
        }

        // ref http://stackoverflow.com/a/3978040
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        private static async Task DoOAuthAsync(string clientId, string clientSecret)
        {
            string state = GenerateRandomDataBase64url(32);
            string codeVerifier = GenerateRandomDataBase64url(32);
            string codeChallenge = Base64UrlEncodeNoPadding(Sha256Ascii(codeVerifier));
            const string codeChallengeMethod = "S256";

            string redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();

            string[] scopes =
            {
                "https://www.googleapis.com/auth/youtube.readonly",
                "https://www.googleapis.com/auth/youtube.force-ssl"
            };

            var scopesString = string.Join(' ', scopes);

            var endpoint = _settings.Get("authorizationEndpoint");

            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
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

            BringConsoleToFront();

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

            await ExchangeCodeForTokensAsync(code, codeVerifier, redirectUri, clientId, clientSecret);
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
        async static Task ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri, string clientId, string clientSecret)
        {
            var tokenRequestUri = "https://www.googleapis.com/oauth2/v4/token";
            var tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectUri),
                clientId,
                codeVerifier,
                clientSecret
                );

            var tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestUri);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var tokenRequestBodyBytes = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = tokenRequestBodyBytes.Length;
            using (Stream requestStream = tokenRequest.GetRequestStream())
            {
                await requestStream.WriteAsync(tokenRequestBodyBytes, 0, tokenRequestBodyBytes.Length);
            }

            try
            {
                var tokenResponse = await tokenRequest.GetResponseAsync();
                using (var reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    var responseText = await reader.ReadToEndAsync();
                    //Console.WriteLine(responseText);

                    var tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    _settings.Set("Token", tokenEndpointDecoded["access_token"]);
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

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">String to be logged</param>

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private static string GenerateRandomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
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

        // Hack to bring the Console window to front.
        // ref: http://stackoverflow.com/a/12066376
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}
