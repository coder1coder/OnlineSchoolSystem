using OnlineSchoolSystem.DataAccess.FileStorage.Models;
using OnlineSchoolSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Если строка является ответом 
        /// </summary>
        /// <returns></returns>
        private bool StringIsAnswer(string stringMessage)
        {
            Regex regex = new Regex(ANSWER_PATTERN, RegexOptions.IgnoreCase);
            return regex.IsMatch(stringMessage);
        }

        /// <summary>
        /// Определить тип сообщения Вопрос, Ответ, обычное сообщение
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public MessageType GetMessageType(Message chatMessage)
        {
            if (MessageIsQuestions(chatMessage))
                return MessageType.Question;

            if (MessageIsAnswer(chatMessage))
                return MessageType.Answer;
            
            return MessageType.Regular;
        }

        /// <summary>
        /// Проверяем является ли сообщение вопросом 
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public bool MessageIsQuestions(Message chatMessage)
        {
            string stringMessage = chatMessage?.Snippet?.TextMessageDetails?.MessageText;
            return StringIsQuestion(stringMessage);
        }

        /// <summary>
        /// Проверяем является ли сообщение ответом
        /// если это так, то вернём вопрос
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <returns></returns>
        public bool MessageIsAnswer(Message chatMessage)
        {
            string stringMessage = chatMessage?.Snippet?.TextMessageDetails?.MessageText;
            return StringIsAnswer(stringMessage);
        }

        public bool IsMessageRegistered()
        {
            return false;
        }

        // по идее этот метод надо вынести в какой то сервис
        public List<Message> GetUniqueValues(List<Message> chatMessages)
        {
            //для сравнения по определённым полям реализовать IEquatable<StubChatMessageEntity> для StubChatMessageEntity
            //https://docs.microsoft.com/ru-ru/dotnet/api/system.linq.enumerable.distinct?view=net-5.0
            var result = chatMessages.Distinct().ToList();
            return result;
        }
    }
}
