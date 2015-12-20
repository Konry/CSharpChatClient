using System;
using System.Net;

namespace CSharpChatClient.Test
{
    using Model;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public partial class TestMessage
    {
        [Test]
        public void TestGenerateConnectMessage()
        {
            User user = new User("TestUser");
            String correctResult = "TCPConnectTo;TestUser;-1;;;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));

            user = new User("TestUser");
            correctResult = "TCPConnectTo;TestUser;-1;;;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 51110));
        }

        [Test]
        public void TestGenerateConnectMessageExtendedUser()
        {
            ExtendedUser exUser = new ExtendedUser(new User("TestUser"));
            exUser.IpAddress = IPAddress.Parse("127.0.2.1");
            exUser.Port = 12345;
            String correctResult = "TCPConnectTo;TestUser;-1;;;127.0.2.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(exUser));

            exUser = new ExtendedUser("TestUser");
            correctResult = "TCPConnectTo;TestUser;-1;;;127.0.0.1;" + Configuration.DEFAULT_TCP_PORT;
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(exUser, IPAddress.Parse("127.0.0.1"), 51110));
        }

        [Test]
        public void TestGenerateConnectMessageUserOnly()
        {
            User user = new User("TestUser");
            String correctResult = "TCPConnectTo;TestUser;-1;;;127.0.0.1;12345";
            Assert.AreEqual(correctResult, Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345));
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
            Assert.AreEqual("TCPMessage", Message.ParseTCPNotifyMessage(Message.GenerateTCPMessage(mess)).MessageType);
            Assert.AreEqual("FromUser", Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.Name);
            Assert.AreEqual(-1, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).FromUser.Id);
            Assert.AreEqual("ToUser", Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.Name);
            Assert.AreEqual(-1, Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).ToUser.Id);
            Assert.AreEqual(("This is the message"), Message.ParseTCPMessage(Message.GenerateTCPMessage(mess)).MessageContent);
        }

        [Test]
        public void TestParseTCPNotify()
        {
            User fromUser = new User("FromUser");
            User toUser = new User("ToUser");
            Message mess = new Message("Rename:123");
            mess.FromUser = fromUser;
            mess.ToUser = toUser;
            Assert.AreEqual("TCPNotify", Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).MessageType);
            Assert.AreEqual("FromUser", Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).FromUser.Name);
            Assert.AreEqual(-1, Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).FromUser.Id);
            Assert.AreEqual(null, Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).ToUser);
            Assert.AreEqual(("Rename:123"), Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).MessageContent);
        }

        [Test]
        public void TestParseTCPConnect()
        {
            User fromUser = new User("FromUser");
            User toUser = new User("ToUser");
            Message mess = new Message("Rename:123");
            mess.FromUser = fromUser;
            mess.ToUser = toUser;
            Assert.AreEqual("TCPNotify", Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).MessageType);
            Assert.AreEqual("FromUser", Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).FromUser.Name);
            Assert.AreEqual(-1, Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).FromUser.Id);
            Assert.AreEqual(null, Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).ToUser);
            Assert.AreEqual(("Rename:123"), Message.ParseTCPNotifyMessage(Message.GenerateTCPNotify(mess)).MessageContent);
        }

        [Test]
        public void TestIsTcpMessage()
        {
            User user = new User("TestUser");
            User user1 = new User("Second");
            Assert.AreEqual(false, Message.IsTCPMessage(Message.GenerateTCPNotify(new Message(user, user1, "Rename"))));
            Assert.AreEqual(true, Message.IsTCPMessage(Message.GenerateTCPMessage(new Message(user, user1, "text"))));
            Assert.AreEqual(false, Message.IsTCPMessage(Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345)));
            Assert.AreEqual(true, Message.IsTCPMessage("TCPMessage;a;1;;;102:192:192:192;12343"));
            Assert.AreEqual(false, Message.IsTCPMessage("Test"));
        }

        [Test]
        public void TestIsNotifyMessage()
        {
            User user = new User("TestUser");
            User user1 = new User("Second");
            Assert.AreEqual(true, Message.IsNotifyMessage(Message.GenerateTCPNotify(new Message(user, user1, "Rename"))));
            Assert.AreEqual(false, Message.IsNotifyMessage(Message.GenerateTCPMessage(new Message(user, user1, "text"))));
            Assert.AreEqual(false, Message.IsNotifyMessage(Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345)));
            Assert.AreEqual(true, Message.IsNotifyMessage("TCPNotify;a;1;;;102:192:192:192;12343"));
            Assert.AreEqual(false, Message.IsNotifyMessage("Test"));
        }

        [Test]
        public void TestIsNewContact()
        {
            User user = new User("TestUser");
            User user1 = new User("Second");
            Assert.AreEqual(false, Message.IsNewContactMessage(Message.GenerateTCPNotify(new Message(user, user1, "Rename"))));
            Assert.AreEqual(false, Message.IsNewContactMessage(Message.GenerateTCPMessage(new Message(user, user1, "text"))));
            Assert.AreEqual(true, Message.IsNewContactMessage(Message.GenerateConnectMessage(user, IPAddress.Parse("127.0.0.1"), 12345)));
            Assert.AreEqual(true, Message.IsNewContactMessage("TCPConnectTo;a;1;;;102:192:192:192;12343"));
            Assert.AreEqual(false, Message.IsNewContactMessage("Test"));
        }

    }
}