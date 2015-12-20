using CSharpChatClient.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Test
{
    [TestFixture]
    class TestMessageHistory
    {

        [Test]
        public void TestFillUpMessageHistory()
        {
            Message messTest = new Message(new User("one", -1),null, "Test");
            Message messFigure = new Message(new User("two", -1), null, "Figure");
            Message messSearch = new Message(new User("one", -1), null, "Search");

            Message[] messArray = new Message[3];
            messArray[0] = messTest;
            messArray[1] = messFigure;
            messArray[2] = messSearch;

            MessageHistory his = new MessageHistory();
            Assert.AreEqual(0, his.ListOfMessages.Count);
            his.FillUpMessageHistory(messArray);
            Assert.AreEqual(3, his.ListOfMessages.Count);
        }

        [Test]
        public void TestAddMessage()
        {
            Message messTest = new Message(new User("one", -1), null, "Test");
            Message messFigure = new Message(new User("two", -1), null, "Figure");
            Message messSearch = new Message(new User("one", -1), null, "Search");

            MessageHistory his = new MessageHistory();
            Assert.AreEqual(0, his.StringBuilder.ToString().Length);
            Assert.AreEqual(0, his.ListOfMessages.Count);
            his.AddMessage(messTest);
            Assert.Greater(his.StringBuilder.ToString().Length, 0);
            Assert.AreEqual(1, his.ListOfMessages.Count);
            his.AddMessage(messFigure);
            Assert.AreEqual(2, his.ListOfMessages.Count);
            his.AddMessage(messSearch);
            Assert.AreEqual(3, his.ListOfMessages.Count);
            Assert.AreEqual("Search", his.ListOfMessages.Last.Value.MessageContent);
        }

        [Test]
        public void TestClearHistory()
        {
            Message messTest = new Message(new User("one", -1), null, "Test");
            Message messFigure = new Message(new User("two", -1), null, "Figure");
            Message messSearch = new Message(new User("one", -1), null, "Search");

            Message[] messArray = new Message[3];
            messArray[0] = messTest;
            messArray[1] = messFigure;
            messArray[2] = messSearch;

            MessageHistory his = new MessageHistory();
            Assert.AreEqual(0, his.ListOfMessages.Count);
            his.FillUpMessageHistory(messArray);
            Assert.AreEqual(3, his.ListOfMessages.Count);
            his.ClearHistory();
            Assert.AreEqual(0, his.ListOfMessages.Count);
        }
    }
}
