using Newtonsoft.Json;
using OnlineSchoolSystem.Utilites;
using OnlineSchoolSystem.YoutubeBot;
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
using System.Threading.Tasks;

namespace OnlineSchoolSystem.Client
{
    class Program
    {
        const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private static string _token;

        static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Required command line arguments: client-id client-secret");
                return 1;
            }
            string clientId = args[0];
            string clientSecret = args[1];

            Console.WriteLine("Добро пожаловать в чат-бот Youtube");
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Для начала работы бота необходимо пройти этап авторизаци");
            Console.WriteLine("Нажмите любую клавишу, чтобы начать..");
            Console.ReadKey();

            Helper.Log("Авторизация", Helper.LogLevel.Info);
            Program p = new Program();
            await p.DoOAuthAsync(clientId, clientSecret);

            if (string.IsNullOrWhiteSpace(_token))
            {
                Helper.Log("Некорректный токен", Helper.LogLevel.Error);
                return 0;
            }

            Helper.Log("Начинаем работу", Helper.LogLevel.Success);

            var bot = new YoutubeBotClient(_token);

            var broadcasts = bot.GetBroadcasts().ToList();

            if (broadcasts.Count() > 0)
            {
                var liveChatId = bot.GetLiveChatIdByBroadcastId(broadcasts.First().Id);

                var messages = bot.GetLiveChatMessages(liveChatId).ToList();

                foreach (var item in messages)
                {
                    Helper.Log(item.ToString(), Helper.LogLevel.Success);
                }
            }
            else
                Helper.Log("Нет существующих трансляций", Helper.LogLevel.Error);

            Console.WriteLine("Нажмите любую клавишу для выхода");
            Console.ReadKey();
            return 0;
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
        private async Task DoOAuthAsync(string clientId, string clientSecret)
        {
            string state = GenerateRandomDataBase64url(32);
            string codeVerifier = GenerateRandomDataBase64url(32);
            string codeChallenge = Base64UrlEncodeNoPadding(Sha256Ascii(codeVerifier));
            const string codeChallengeMethod = "S256";

            string redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            http.Start();

            string authorizationRequest = string.Format("{0}?response_type=code&scope=https://www.googleapis.com/auth/youtube.readonly&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                AuthorizationEndpoint,
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

        async Task ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri, string clientId, string clientSecret)
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

                    _token = tokenEndpointDecoded["access_token"];
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

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}
