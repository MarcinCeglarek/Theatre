﻿namespace Theatre.Common.Tests.Agents
{
    #region Usings

    using System;

    using Akka.Actor;
    using Akka.TestKit.Xunit2;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Theatre.Common.Agents;
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
                    this.target.Tell(new FileHashed("Path", 10, new byte[] { 0, 1, 2, 3 }, DateTime.Now, DateTime.Now)));
        }
    }
}