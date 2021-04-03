using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Newtonsoft.Json;

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
            var chatMessages = ReadAllMessagesFromFile();
            chatMessages.Add(chatMessage);

            WriteMessagesToFile(chatMessages);
        }

        // Записать в файл сообщения
        public void AppendMessagesToFile(List<StubChatMessageEntity> messages)
        {
            var storedChatMessages = ReadAllMessagesFromFile();
            storedChatMessages.AddRange(messages);
            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщение
        public void DeleteMessage(StubChatMessageEntity messages)
        {
            // найти и удалить по совпадению чего?
            throw new NotImplementedException();
        }

        // Удалить сообщения
        public void DeleteMessages(List<StubChatMessageEntity> messages)
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
                return chatMessages;
            }
            else
            {
                return new List<StubChatMessageEntity>();
            }
        }
    }
}
