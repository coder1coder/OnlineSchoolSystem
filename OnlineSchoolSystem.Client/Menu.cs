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
            Helper.Log("Добро пожаловать в приложение!");
            Helper.Log("1) Подключиться к стриму");
            Helper.Log("2) Посмотреть лог сообщений с предыдущих стримов");
            Helper.Log("3) Сформировать статистику");
            Helper.Log("0) Выход");
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
                Helper.Log(question);
                answer = Console.ReadLine();
            }
            return answer;
        }
    }
}
