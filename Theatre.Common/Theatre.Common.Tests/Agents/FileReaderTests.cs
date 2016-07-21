using System.IO;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class FileReaderTests : TestKit
    {
        private const string NonExistentPath = "not/existent/path";

        private const string ExistentPath = "";

        private IActorRef _target;


        [TestInitialize]
        public void BeforeTest()
        {
            _target = ActorOf(Props.Create(() => new FileReader(ActorOf(BlackHoleActor.Props))));
        }

        [TestMethod]
        public void LogsInfoWhenReceivedFile()
        {
            EventFilter.Info($"Processing {NonExistentPath}").ExpectOne(() => _target.Tell(new HashFile(NonExistentPath)));
        }

        [TestMethod]
        public void LogsWarningWhenFileNotFound()
        {
            EventFilter.Warning($"{NonExistentPath}: File not found").ExpectOne(() => _target.Tell(new HashFile(NonExistentPath)));
        }

        [TestMethod]
        public void LogsDebugWhenStartingToReadFile()
        {
            EventFilter.Debug($"Reading {ExistentPath}").ExpectOne(() => _target.Tell(new HashFile(ExistentPath)));
        }
    }
}