using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.Menu
{
    internal class Menu
    {
        public void Start()
        {
            var isContinue = true;
            do
            {
                Console.WriteLine("Добро пожаловать в приложение!");
                Console.WriteLine("1) Подключиться к стриму");
                Console.WriteLine("2) Посмотреть лог сообщений с предыдущих стримов");
                Console.WriteLine("3) Сформировать статистику");
                Console.WriteLine("0) Выход");
                Console.Write("Введите номер операции, которую хотите совершить: ");

                string operation = Console.ReadLine();
                switch (operation)
                {
                    case "1":
                        {
                            ConnectToStreame();
                            break;
                        }
                    case "2":
                        {
                            GetMessagesFromPreviousStream();
                            break;
                        }
                    case "3":
                        {
                            GetStatistic();
                            break;
                        }
                    case "0":
                        {
                            isContinue = false;
                            Console.WriteLine("Выход");
                            break;
                        }
                    default:
                        break;
                }

            } while (isContinue);
        }

        private void GetStatistic()
        {
            throw new NotImplementedException();
        }

        private void GetMessagesFromPreviousStream()
        {
            ShowIdPreviousStream();
        }

        private void ConnectToStreame()
        {
            var ClientId = GetAnswer("Введите ClientId: ");
            var clientSecret = GetAnswer("Введите ClientSecret: ");
            Console.WriteLine("Подключились к стриму с ClientId и ClientSecret");
        }
        private string GetAnswer(string question)
        {
            string answer = null;
            while (answer == null)
            {
                Console.Write(question);
                answer = Console.ReadLine();
            }
            return answer;
        }
    }
}
