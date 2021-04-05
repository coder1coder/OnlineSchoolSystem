using OnlineSchoolSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineSchoolSystem.DataAccess.FileStorage.Models
{
    public class MessageStoreModel
    {
        public MessageType messageType { get; set; }
        public Message message { get; set; }
    }
}
