using CSharpChatClient.Model;
using NUnit.Framework;
using System.Net;

namespace CSharpChatClient.Test
{
    [TestFixture]
    class TestExtendedUser
    {
        
        [Test]
        public void TestParseFromMessage()
        {
            string correctResult = "TCPConnectTo;TestUser;-1;;;127.0.0.1;12345";

            ExtendedUser exUserToTest = new ExtendedUser(new User("Tom",-1),IPAddress.Parse("192.168.23.1"),1234);
            Message message = new Message("192.168.23.1;1234");
            message.FromUser = new User("Tom", -1);
            message.ToUser = null;
            Assert.AreEqual(exUserToTest.Name, ExtendedUser.ParseFromMessage(message).Name);
            Assert.AreEqual(exUserToTest.Id, ExtendedUser.ParseFromMessage(message).Id);
            Assert.AreEqual(exUserToTest.IpAddress, ExtendedUser.ParseFromMessage(message).IpAddress);
            Assert.AreEqual(exUserToTest.Port, ExtendedUser.ParseFromMessage(message).Port);
            Assert.True(exUserToTest.Equals(exUserToTest));
        }

        [Test]
        public void TestEquals()
        {
            ExtendedUser exUserToTest = new ExtendedUser(new User("Tom", -1), IPAddress.Parse("192.168.23.1"), 1234);
            ExtendedUser exUserEqual = new ExtendedUser(new User("Tom", -1), IPAddress.Parse("192.168.23.1"), 1234);
            ExtendedUser exUserUnequalName = new ExtendedUser(new User("Tomi", -1), IPAddress.Parse("192.168.23.1"), 1234);
            ExtendedUser exUserUnequalId = new ExtendedUser(new User("Tom", 3), IPAddress.Parse("192.168.23.1"), 1234);
            ExtendedUser exUserUnequalIp = new ExtendedUser(new User("Tom", -1), IPAddress.Parse("192.168.26.1"), 1234);
            ExtendedUser exUserUnequalPort = new ExtendedUser(new User("Tom", -1), IPAddress.Parse("192.168.23.1"), 5234);

            Assert.True(exUserToTest.Equals(exUserEqual));
            Assert.False(exUserToTest.Equals(exUserUnequalName));
            Assert.False(exUserToTest.Equals(exUserUnequalId));
            Assert.True(exUserToTest.Equals(exUserUnequalIp));
            Assert.True(exUserToTest.Equals(exUserUnequalPort));
        }
    }
}
