using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OnlineSchoolSystem.DataAccess.FileStorage;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.Utilites;

namespace OnlineSchoolSystem.Menu
{
    internal class Menu
    {
        public void Start()
        {
            var isContinue = true;
            do
            {
                Helper.Log("Добро пожаловать в приложение!");
                Helper.Log("1) Подключиться к стриму");
                Helper.Log("2) Посмотреть лог сообщений с предыдущих стримов");
                Helper.Log("3) Сформировать статистику");
                Helper.Log("0) Выход");
                Helper.Log("Введите номер операции, которую хотите совершить: ");

                string operation = Console.ReadLine();
                switch (operation)
                {
                    case Operations.CONNECT_TO_STREAM:
                        {
                            ConnectToStream();
                            break;
                        }
                    case Operations.GET_MESSAGES_FROM_PREVIOUS_STREAMS:
                        {
                            GetMessagesFromPreviousStreams();
                            break;
                        }
                    case Operations.GET_STATISTIC:
                        {
                            GetStatistic();
                            break;
                        }
                    case Operations.EXIT:
                        {
                            isContinue = false;
                            Helper.Log("Выход");
                            break;
                        }
                    default:
                        break;
                }

            } while (isContinue);
        }

        private void GetStatistic()
        {
            //будет формироваться список авторов с количеством вопросов и ответов для одного стрима             GetReport();
            //будет формироваться список вопросов и ответов для определенного автора по одному стриму. GetAuthorReport(AuthorDetails author) .
            GetUserAnswer("Введите ид стрима для получения статистики");
            var idStream = Console.ReadLine();
            var report = new StatisticPerStream();
            report.LiveChatId = idStream;
            report.Answers = GetAnsweers(idStream);
            report.Authors = GetAuthors(idStream);
            report.Questions = GetQuestions(idStream);
        }
        //перенести в клиента
        //private List<IQuestion> GetQuestions(string idStream)
        //{
        //    throw new NotImplementedException();
        //}

        //private List<AuthorDetails> GetAuthors(string idStream)
        //{
        //    throw new NotImplementedException();
        //}
        ///// <summary>
        ///// Получает список ответов со стрима с идентификатором idStream
        ///// </summary>
        ///// <param name="idStream"></param>
        ///// <returns></returns>
        //private List<IAnswer> GetAnsweers(string idStream)
        //{
        //    var answers = new List<IAnswer>();
        //    return answers;
        //}

        /// <summary>
        /// Получение имени файла по идентификатору стрима
        /// </summary>
        /// <param name="idStream"></param>
        /// <returns></returns>
        private string GetFileName(string idStream)
        {
            throw new NotImplementedException();
        }

        private string GetCurrentFile()
        {
            throw new NotImplementedException();
        }

        private void GetMessagesFromPreviousStreams()
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
        /// <summary>
        /// Подключение к стриму. Добавить метод подключения с полученными параметрами
        /// </summary>
        private void ConnectToStream()
        {
            var clientId = GetUserAnswer("Введите ClientId: ");
            var clientSecret = GetUserAnswer("Введите ClientSecret: ");
            Helper.Log("Подключились к стриму с ClientId и ClientSecret");

        }
        /// <summary>
        /// Получает от пользователя ответ на вопрос question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private string GetUserAnswer(string question)
        {
            string answer = null;
            while (answer == null)
            {
                Helper.Log(question);
                answer = Console.ReadLine();
            }
            return answer;
        }
    }
}
