namespace Theatre.Common.Tests.Actors
{
    #region Usings

    using System;

    using Akka.Actor;
    using Akka.TestKit.Xunit2;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Theatre.Common.Actors;
    using Theatre.Common.Messages;

    #endregion

    [TestClass]
    public class DatabaserTests : TestKit
    {
        private IActorRef target;

        public DatabaserTests()
            : base(@"akka.loglevel = DEBUG")
        {
        }

        [TestMethod]
        public void LogsFileHashedMessage()
        {
            this.target = this.ActorOf(Props.Create(() => new Databaser()));
            this.EventFilter.Info("Path hashed")
                .ExpectOne(
                    () =>
                    this.target.Tell(new FileProcessed("Path", 10, new byte[] { 0, 1, 2, 3 }, DateTime.Now, DateTime.Now)));
        }
    }
}