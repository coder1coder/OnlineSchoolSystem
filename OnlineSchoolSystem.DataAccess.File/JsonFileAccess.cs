using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    //https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-core-3-1
    public class JsonFileAccess
    {
        private readonly string fileName;
        public JsonFileAccess(string fileName)
        {
            this.fileName = fileName;
        }

        // записываем сообщения в файлы
        private void WriteMessagesToFile(List<StubChatMessageEntity> chatMessages)
        {
            string jsonString = JsonConvert.SerializeObject(chatMessages, Formatting.Indented);

            File.WriteAllText(fileName, jsonString);
        }

        // Записать в файл сообщение
        public void AppendMessageToFile(StubChatMessageEntity chatMessage)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.Add(chatMessage);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Записать в файл сообщения
        public void AppendMessagesToFile(List<StubChatMessageEntity> chatMessages)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.AddRange(chatMessages);
            storedChatMessages = GetUniqueValues(storedChatMessages);

            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщение
        public void DeleteMessage(StubChatMessageEntity chatMessages)
        {
            // найти и удалить по совпадению чего?
            throw new NotImplementedException();
        }

        // Удалить сообщения
        public void DeleteMessages(List<StubChatMessageEntity> chatMessages)
        {
            // найти и удалить по совпадению чего?
            throw new NotImplementedException();
        }

        // Прочитать из файла
        public List<StubChatMessageEntity> ReadAllMessagesFromFile()
        {
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                var chatMessages = JsonConvert.DeserializeObject<List<StubChatMessageEntity>>(json);
                if (chatMessages == null)
                    return new List<StubChatMessageEntity>();
                return chatMessages;
            }
            else
            {
                return new List<StubChatMessageEntity>();
            }
        }

        public List<StubChatMessageEntity> GetUniqueValues(List<StubChatMessageEntity> chatMessages)
        {
            //для сравнения по определённым полям реализовать IEquatable<StubChatMessageEntity> для StubChatMessageEntity
            //https://docs.microsoft.com/ru-ru/dotnet/api/system.linq.enumerable.distinct?view=net-5.0
            var result = chatMessages.Distinct().ToList();
            return result;
        }
    }

   
}
