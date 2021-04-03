using System;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    //заглушка для сохранения сообщения из чата. Заменим в рабочем коде
    public class StubChatMessageEntity
    {
        public int Id { get; set; }
        public DateTime MessageDateSend { get; set; }
        public string Message { get; set; }
    }
}
