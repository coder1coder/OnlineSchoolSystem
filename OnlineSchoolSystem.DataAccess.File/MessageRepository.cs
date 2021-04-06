using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.DataAccess.FileStorage.Models;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _fileName;
        private readonly MessageService _messageService;
             
        public MessageRepository(string fileName)
        {
            this._fileName = fileName;
            _messageService = new MessageService();
        }

        // записываем сообщения в файлы
        private void WriteMessagesToFile(List<Message> chatMessages)
        {
            string jsonString = JsonConvert.SerializeObject(chatMessages, Formatting.Indented);

            File.WriteAllText(_fileName, jsonString);
        }

        // Записать в файл одно сообщение
        public void AddMessage(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.Add(chatMessage);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Записать в файл сообщения
        public void AddMessages(List<Message> chatMessages)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.AddRange(chatMessages);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщение
        public void DeleteMessage(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.Remove(chatMessage);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщения
        public void DeleteMessages(List<Message> chatMessages)
        {
            var storedChatMessages = ReadAllMessages();
            foreach (var item in chatMessages)
            {
                storedChatMessages.Remove(item);
            }
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Прочитать из файла
        public List<Message> ReadAllMessages()
        {
            if (File.Exists(_fileName))
            {
                string json = File.ReadAllText(_fileName);
                var chatMessages = JsonConvert.DeserializeObject<List<Message>>(json);
                if (chatMessages == null)
                    return new List<Message>();
                return chatMessages;
            }
            else
            {
                return new List<Message>();
            }
        }

        public List<Message> GetAllQuestions()
        {
            return null;
        }

        public List<Message> GetAllAnswers()
        {
            return null;
        }

        // по идее этот метод надо вынести в какой то сервис
        public List<Message> GetUniqueValues(List<Message> chatMessages)
        {
            //для сравнения по определённым полям реализовать IEquatable<StubChatMessageEntity> для StubChatMessageEntity
            //https://docs.microsoft.com/ru-ru/dotnet/api/system.linq.enumerable.distinct?view=net-5.0
            var result = chatMessages.Distinct().ToList();
            return result;
        }

        public List<MessageStoreModel> GetMessagesType(List<Message> messages)
        {
            List<MessageStoreModel> result = new List<MessageStoreModel>();
            foreach (var message in messages)
            {
                
                _messageService.
            }

        }
    }


}
