using System;
using System.Diagnostics.CodeAnalysis;

namespace OnlineSchoolSystem.DataAccess.FileStorage
{
    //заглушка для сохранения сообщения из чата. Заменим в рабочем коде и не забудем про реализацию IEquatable<StubChatMessageEntity>
    public class StubChatMessageEntity: IEquatable<StubChatMessageEntity>
    {
        public int Id { get; set; }
        public DateTime MessageDateSend { get; set; }
        public string Message { get; set; }

        public bool Equals(StubChatMessageEntity other)
        {
            // Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return Id.Equals(other.Id) && Message.Equals(other.Message);
        }

        public override int GetHashCode()
        {
            int hashProductId = Id.GetHashCode();
            int hashProductMessage = Message == null ? 0 : Message.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductId ^ hashProductMessage;
        }
    }
}
