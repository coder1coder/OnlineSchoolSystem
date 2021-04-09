using OnlineSchoolSystem.Utilites;
using System;

namespace OnlineSchoolSystem.Client
{
    public class Menu
    {

        public bool IsContinue { get; set; } = true;

        /// <summary>
        /// Display menu items 
        /// </summary>
        public void PrintMenu()
        {
            Console.Clear();
            Helper.Print("Добро пожаловать в приложение!");
            Helper.Print("1) Подключиться к стриму");
            Helper.Print("2) Посмотреть лог сообщений с предыдущих стримов");
            Helper.Print("3) Сформировать статистику");
            Helper.Print("0) Выход");
        }

        /// <summary>
        /// Get user operation
        /// </summary>
        /// <returns></returns>
        public OperationsEnum GetSelectedOperation()
        {
            int operation;

            do
            {
                Console.Write("Введите номер операции, которую хотите совершить: ");
            }
            while (int.TryParse(Console.ReadLine(), out operation) == false);
            
            Console.WriteLine();

            return (OperationsEnum)operation;
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
        /// Получает от пользователя ответ на вопрос question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public string GetUserAnswer(string question)
        {
            string answer = null;
            while (answer == null)
            {
                Helper.Print(question);
                answer = Console.ReadLine();
            }
            return answer;
        }
    }
}
