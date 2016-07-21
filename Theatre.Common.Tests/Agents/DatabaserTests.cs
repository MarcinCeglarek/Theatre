using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class DatabaserTests : TestKit
    {
        private IActorRef _target;

        [TestMethod]
        public void LogsFileHashedMessage()
        {
            _target = ActorOf(Props.Create(() => new Databaser()));
            EventFilter.Info("Path hashed")
                .ExpectOne(
                    () => _target.Tell(new FileHashed("Path", 10, new byte[] {0, 1, 2, 3}, DateTime.Now, DateTime.Now)));

        }
    }
}