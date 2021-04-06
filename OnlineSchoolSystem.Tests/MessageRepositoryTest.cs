using NUnit.Framework;
using OnlineSchoolSystem.DataAccess.FileStorage;
using OnlineSchoolSystem.Models;
using System.Collections.Generic;
using AutoFixture;
using System.IO;

namespace OnlineSchoolSystem.Tests
{
    public class MessageStoreTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddMessages_shouldStoreData()
        {
            // arrange
            string fileName = "data_for_test.json";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            MessageStore messageStore = new MessageStore(fileName);
            List<Message> messages = new List<Message>()
            {
                new Message()
                {
                    Id = new Fixture().Create<string>(),
                    Snippet = new Snippet()
                    {
                        TextMessageDetails = new TextMessageDetails()
                        {
                            MessageText = "Q: Вопрос 1"
                        }
                    }
                }
            };
            // act
            messageStore.AddMessages(messages);
            List<Message> messages1 = messageStore.ReadAllMessages();

            // assert 
            Assert.AreEqual(messages.Count, messages1.Count);

        }
    }
}