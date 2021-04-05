using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using OnlineSchoolSystem.Models;
using System.Linq;
using OnlineSchoolSystem.Bots.Youtube.Models;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    public class JsonFileAccess: IStorage
    {
        private readonly string _fileName;
        public JsonFileAccess(string fileName)
        {
            this._fileName = fileName;
        }

        // записываем сообщения в файлы
        private void WriteMessagesToFile(List<Message> chatMessages)
        {
            string jsonString = JsonConvert.SerializeObject(chatMessages, Formatting.Indented);

            File.WriteAllText(_fileName, jsonString);
        }

        // Записать в файл сообщение
        public void AppendMessageToFile(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.Add(chatMessage);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Записать в файл сообщения
        public void AppendMessagesToFile(List<Message> chatMessages)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.AddRange(chatMessages);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщение
        public void DeleteMessage(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.Remove(chatMessage);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщения
        public void DeleteMessages(List<Message> chatMessages)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            foreach (var item in chatMessages)
            {
                storedChatMessages.Remove(item);
            }
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Прочитать из файла
        public List<Message> ReadAllMessagesFromFile()
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

        public List<Message> GetUniqueValues(List<Message> chatMessages)
        {
            //для сравнения по определённым полям реализовать IEquatable<StubChatMessageEntity> для StubChatMessageEntity
            //https://docs.microsoft.com/ru-ru/dotnet/api/system.linq.enumerable.distinct?view=net-5.0
            var result = chatMessages.Distinct().ToList();
            return result;
        }
    }

   
}
