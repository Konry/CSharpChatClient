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
            String correctResult = "TRYConnectTo;TestUser;-1;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));

            user = new User("TestUser");
            correctResult = "TRYConnectTo;TestUser;-1;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 51110));
        }

        [Test]
        public void TestGenerateConnectMessageUserOnly()
        {
            User user = new User("TestUser");
            String correctResult = "TRYConnectTo;TestUser;-1;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));

            //user = new User("TestUser");
            //correctResult = "TRYConnectTo;TestUser;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            //Assert.AreEqual(correctResult, NetworkMessage.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));
        }

        [Test]
        public void TestHeartbeatMessage()
        {
            User user = new User("TestUser");
            String correctResult = "Heartbeat;TestUser;-1;127.0.0.1;12345;live";
            Assert.AreEqual(correctResult, Message.GenerateHeartbeatMessage(user, IPAddress.Parse("127.0.0.1"), 12345, true));
            correctResult = "Heartbeat;TestUser;-1;127.0.0.1;12345;offline";
            Assert.AreEqual(correctResult, Message.GenerateHeartbeatMessage(user, IPAddress.Parse("127.0.0.1"), 12345, false));
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
            String correctResult = "TCPMessage;FromUser;-1;ToUser;-1;This is the message";
            Debug.WriteLine(Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).MessageContent);
            Assert.AreEqual(mess.FromUser.name, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.name);
            Assert.AreEqual(mess.FromUser.id, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.id);
            Assert.AreEqual(mess.ToUser.name, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.name);
            Assert.AreEqual(mess.ToUser.id, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.id);
            Assert.AreEqual(mess.MessageContent, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).MessageContent);
        }

        [Test]
        public void TestIsNewContact()
        {
            User user = new User("TestUser");
            Assert.AreEqual(true, Message.IsNewContact(Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345)));
            Assert.AreEqual(false, Message.IsNewContact("Test"));
        }


    }
}
