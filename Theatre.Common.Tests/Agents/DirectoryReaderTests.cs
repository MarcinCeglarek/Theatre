using System.IO;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class DirectoryReaderTests : TestKit
    {
        private readonly string nonExistentPath = "non/existent/path";
        private string _existentPath;

        private IActorRef _target;
        private TestProbe _testProbe;

        [TestInitialize]
        public void BeforeTest()
        {
            _existentPath = Directory.GetCurrentDirectory();
            _testProbe = this.CreateTestProbe();
            _target = ActorOf(Props.Create(() => new DirectoryReader(_testProbe)));
        }

        [TestMethod]
        public void StartsProcessingDirectory()
        {
            EventFilter.Info("Processing " + nonExistentPath)
                .ExpectOne(() => _target.Tell(new HashDirectory(nonExistentPath)));
        }

        [TestMethod]
        public void LogsWarningIfDirectoryDoesNotExists()
        {
            EventFilter.Warning(nonExistentPath + ": Dir not found")
                .ExpectOne(() => _target.Tell(new HashDirectory(nonExistentPath)));
        }

        [TestMethod]
        public void LogsInfoWhenStartingProcessingDirectory()
        {
            EventFilter.Info("Reading " + _existentPath).ExpectOne(() => _target.Tell(new HashDirectory(_existentPath)));
        }

        [TestMethod]
        public void SendsMessageWhenFinishedReadingDirectory()
        {
            _target.Tell(new HashDirectory(_existentPath));
            _testProbe.ExpectMsg<DirectoryHashed>(message => message.FullPath == _existentPath);
        }
    }
}