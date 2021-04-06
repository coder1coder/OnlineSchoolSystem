using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using OnlineSchoolSystem.Models;
using OnlineSchoolSystem.DataAccess.FileStorage.Models;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    public class MessageStore : IMessageStore
    {
        private readonly string _fileName;
        private readonly MessageService _messageService;

        public MessageStore(string fileName)
        {
            this._fileName = fileName;
            _messageService = new MessageService();
        }

        // преобразовать в модель для сохранения
        private List<MessageForStore> ConvertMessagesForStore(List<Message> chatMessages)
        {
            List<MessageForStore> messagesForStore = new List<MessageForStore>();
            foreach (var chatMessage in chatMessages)
            {
                messagesForStore.Add(
                    new MessageForStore()
                    {
                        message = chatMessage,
                        messageType = _messageService.GetMessageType(chatMessage)
                    }
                );
            }

            return messagesForStore;
        }

        // преоразовать из модели для сохранения в сообщения
        private List<Message> ConvertToMessageFromStore(List<MessageForStore> storedMessages)
        {
            List<Message> messages = new List<Message>();
            foreach (var storedMessage in storedMessages)
            {
                messages.Add(storedMessage.message);
            }

            return messages;
        }

        // записываем сообщения в файлы
        private void WriteMessagesToFile(List<Message> chatMessages)
        {
            //Только уникальные значения
            chatMessages = _messageService.GetUniqueValues(chatMessages);
            var messagesForStore = ConvertMessagesForStore(chatMessages);

            string jsonString = JsonConvert.SerializeObject(messagesForStore, Formatting.Indented);
            File.WriteAllText(_fileName, jsonString);
        }

        // Записать в файл одно сообщение
        public void AddMessage(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.Add(chatMessage);
            WriteMessagesToFile(storedChatMessages);
        }

        // Записать в файл сообщения
        public void AddMessages(List<Message> chatMessages)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.AddRange(chatMessages);
            WriteMessagesToFile(storedChatMessages);
        }

        // Удалить сообщение
        public void DeleteMessage(Message chatMessage)
        {
            var storedChatMessages = ReadAllMessages();
            storedChatMessages.Remove(chatMessage);
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
            WriteMessagesToFile(storedChatMessages);
        }

        // Прочитать из файла
        public List<Message> ReadAllMessages()
        {
            if (File.Exists(_fileName))
            {
                string json = File.ReadAllText(_fileName);
                var storedChatMessages = JsonConvert.DeserializeObject<List<MessageForStore>>(json);
                var chatMessages = ConvertToMessageFromStore(storedChatMessages);
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
            if (File.Exists(_fileName))
            {
                string json = File.ReadAllText(_fileName);
                var storedChatMessages = JsonConvert.DeserializeObject<List<MessageForStore>>(json);
                storedChatMessages = storedChatMessages.Where(m => m.messageType == MessageType.Question).ToList();
                var chatMessages = ConvertToMessageFromStore(storedChatMessages);
                if (chatMessages == null)
                    return new List<Message>();
                return chatMessages;
            }
            else
            {
                return new List<Message>();
            }
        }

        public List<Message> GetAllAnswers()
        {
            if (File.Exists(_fileName))
            {
                string json = File.ReadAllText(_fileName);
                var storedChatMessages = JsonConvert.DeserializeObject<List<MessageForStore>>(json);
                storedChatMessages = storedChatMessages.Where(m => m.messageType == MessageType.Answer).ToList();
                var chatMessages = ConvertToMessageFromStore(storedChatMessages);
                if (chatMessages == null)
                    return new List<Message>();
                return chatMessages;
            }
            else
            {
                return new List<Message>();
            }
        }
    }
}
