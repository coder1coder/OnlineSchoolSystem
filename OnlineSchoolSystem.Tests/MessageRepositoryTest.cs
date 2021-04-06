using NUnit.Framework;
using OnlineSchoolSystem.DataAccess.FileStorage;
using OnlineSchoolSystem.Models;
using System.Collections.Generic;
using AutoFixture;

namespace OnlineSchoolSystem.Tests
{
    public class MessageStoreTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            // arrange
            string fileName = "data.json";
            MessageStore messageStore = new MessageStore(fileName);
            List<Message> messages = new List<Message>()
            {
                new Message()
                {
                    Id = new Fixture().Create<string>(),
                    Snippet = new Snippet()
                    {
                        
                    }
                }
            };
            messageStore.AddMessages( messages);

            // act

            // assert 
        }
    }
}