using OnlineSchoolSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    public class MessageService
    {
        // регистрируем сообщение, является ли оно вопросом или ответом
        public void RegisterMessage()
        {

        }

        // Q: {message}
        const string QUESTION_PATTERN = @"Q:(\w*)";
        // A#{id}: {message}
        const string ANSWER_PATTERN = @"A#(\d*):(\w*)";

        /// <summary>
        /// Является ли строка вопросом
        /// </summary>
        /// <returns></returns>
        private bool StringIsQuestion(string stringMessage)
        {
            Regex regex = new Regex(QUESTION_PATTERN, RegexOptions.IgnoreCase);
            return regex.IsMatch(stringMessage);
        }

        /// <summary>
        /// Если строка является ответом то получить номер вопроса, если нет, то 0
        /// </summary>
        /// <returns></returns>
        private int StringIsAnswer(string stringMessage)
        {
            Regex regex = new Regex(ANSWER_PATTERN, RegexOptions.IgnoreCase);
            if (regex.IsMatch(stringMessage))
            {
                string questionNumberString = Regex.Match(stringMessage, @"\d+").Value;
                int questionNumber = Int32.Parse(questionNumberString);

                return questionNumber;
            }
            else
                return 0;
        }

        /// <summary>
        /// Проверяем является ли сообщение вопросом и если это так, то вернём true
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public bool MessageIsQuestions(Message chatMessage)
        {
            string stringMessage = chatMessage?.Snippet?.TextMessageDetails?.MessageText;
            return StringIsQuestion(stringMessage);
        }

        /// <summary>
        /// Проверяем является ли сообщение ответом на какой либо вопрос, 
        /// если это так, то вернём вопрос
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public Message MessageIsAnswer(Message chatMessage)
        {
            string stringMessage = chatMessage?.Snippet?.TextMessageDetails?.MessageText;
            int QuestionId = StringIsAnswer(stringMessage);
            // TODO: найти сообщение по ответу

            return null;
        }

        public bool IsMessageRegistered()
        {
            return false;
        }
    }
}
