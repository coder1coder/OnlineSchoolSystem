using Newtonsoft.Json;
using OnlineSchoolSystem.Bots;
using OnlineSchoolSystem.Bots.Models;
using OnlineSchoolSystem.Bots.Youtube.Models;
using OnlineSchoolSystem.DataAccess.FileStorage;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.Utilites;
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

namespace OnlineSchoolSystem.Client
{
    class Program
    {
        private static ISettings _settings;
        private static List<IBot> _bots;

        static void Main(string[] args)
        {
            _settings = new JsonSettings(Environment.CurrentDirectory + "/settings.json");
            _bots = new List<IBot>
            {
                new YoutubeBot()
            };

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

                        _bots
                            .OfType<YoutubeBot>()
                            .FirstOrDefault()?
                            .StartAsync(
                                new JsonFileAccess(_settings.Get("storageDirectoryName")),
                                _settings
                            );

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
                            break;
                        }
                    default:
                        Helper.Log("Некорректная операция", Helper.LogLevel.Error);
                        break;
                }

            } while (menu.IsContinue);

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
                Members = GetMembers(idStream),
                Questions = GetQuestions(idStream)
            };
        }

        private static List<IAnswer> GetAnswers(string idStream)
        {
            throw new NotImplementedException();
        }

        private static List<Member> GetMembers(string idStream)
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
            var messages = new List<Models.Message>();
            foreach (var file in files)
            {
                var fileAccess = new JsonFileAccess(file);
                messages.AddRange(fileAccess.ReadAllMessagesFromFile());
            }
            // треубется реализовать получение из списка сообщений только те сообщения, у которых  message type is textMessageEvent.(Linq выражением)
            foreach (var message in messages)
            {
                Helper.Log(message.Member + "  " + message.Text);
            }
        }
    }
}
