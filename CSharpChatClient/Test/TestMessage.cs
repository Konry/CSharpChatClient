using System;
using System.Net;

namespace CSharpChatClient.Test
{
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public partial class TestMessage
    {
        [Test]
        public void TestGenerateConnectMessage()
        {
            User user = new User("TestUser");
            String correctResult = "TCPConnectTo;TestUser;-1;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));

            user = new User("TestUser");
            correctResult = "TCPConnectTo;TestUser;-1;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 51110));
        }

        [Test]
        public void TestGenerateConnectMessageUserOnly()
        {
            User user = new User("TestUser");
            String correctResult = "TCPConnectTo;TestUser;-1;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));

            //user = new User("TestUser");
            //correctResult = "TCPConnectTo;TestUser;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            //Assert.AreEqual(correctResult, NetworkMessage.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));
        }

        [Test]
        public void TestGenerateTCPMessage()
        {
            User fromUser = new User("FromUser");
            User toUser = new User("ToUser");
            Message mess = new Message("This is the message");
            mess.FromUser = fromUser;
            mess.ToUser = toUser;
            String correctResult = "TCPMessage;FromUser;-1;ToUser;-1;This is the message";
            Assert.AreEqual(correctResult, Message.GenerateTCPMessage(mess));
        }

        [Test]
        public void TestParseTCPMessage()
        {
            User fromUser = new User("FromUser");
            User toUser = new User("ToUser");
            Message mess = new Message("This is the message");
            mess.FromUser = fromUser;
            mess.ToUser = toUser;
            //String correctResult = "TCPMessage;FromUser;-1;ToUser;-1;This is the message";
            Debug.WriteLine(Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).MessageContent);
            Assert.AreEqual(mess.FromUser.Name, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.Name);
            Assert.AreEqual(mess.FromUser.Id, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.Id);
            Assert.AreEqual(mess.ToUser.Name, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.Name);
            Assert.AreEqual(mess.ToUser.Id, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.Id);
            Assert.AreEqual(mess.MessageContent, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).MessageContent);
        }

        [Test]
        public void TestIsNewContact()
        {
            User user = new User("TestUser");
            Assert.AreEqual(true, Message.IsNewContactMessage(Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345)));
            Assert.AreEqual(false, Message.IsNewContactMessage("Test"));
        }
    }
}