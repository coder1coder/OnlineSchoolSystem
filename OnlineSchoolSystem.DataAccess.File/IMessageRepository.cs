using OnlineSchoolSystem.Models;
using System.Collections.Generic;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    public interface IMessageRepository
    {
        void AddMessage(Message chatMessage);
        void AddMessages(List<Message> chatMessages);
        void DeleteMessage(Message chatMessage);
        void DeleteMessages(List<Message> chatMessages);
        List<Message> GetUniqueValues(List<Message> chatMessages);
        List<Message> ReadAllMessages();
    }
}