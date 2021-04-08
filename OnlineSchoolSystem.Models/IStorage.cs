using System.Collections.Generic;

namespace OnlineSchoolSystem.Models
{
    public interface IStorage
    {
        void Store(IEnumerable<Message> messages);
    }
}
