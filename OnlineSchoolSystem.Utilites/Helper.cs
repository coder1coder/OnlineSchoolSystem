using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace OnlineSchoolSystem.Utilites
{
    public static class Helper
    {
        public enum LogLevel { Success, Info, Warning, Error, Default };

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        internal static string GenerateRandomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlEncodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string, which is assumed to be ASCII.
        /// </summary>
        internal static byte[] Sha256Ascii(string text)
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
        internal static string Base64UrlEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        public static void PressAnyKeyToContinue()
        {
            Print("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
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

        /// <summary>
        /// Логирует сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="level">Уровень сообщения</param>
        public static void Log(string message, 
            LogLevel level = LogLevel.Default,
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string caller = "", 
            string pattern = "{2} {1}: {0}")
        {
            var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
            var output = string.Format(pattern, message, caller, callerTypeName);

            switch (level)
            {
                case LogLevel.Success:
                    Print(output, ConsoleColor.Green);
                    break;
                case LogLevel.Info:
                    Print(output, ConsoleColor.Cyan);
                    break;
                case LogLevel.Warning:
                    Print(output, ConsoleColor.Yellow);
                    break;
                case LogLevel.Error:
                    Print(output, ConsoleColor.Red);
                    break;
                default:
                    Print(output);
                    break;
            }
        }

        public static void Print(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }
    }
}
