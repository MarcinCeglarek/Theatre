using System.IO;
using SystemWrapper.IO;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class FileReaderTests : TestKit
    {
        private const string NonExistentPath = "not/existent/path";

        private string _existentPath;

        private IActorRef _target;

        private TestProbe _testProbe;

        [TestInitialize]
        public void BeforeTest()
        {
            _existentPath = Directory.GetCurrentDirectory() + "\\Theatre.Common.Tests.dll";
            _testProbe = CreateTestProbe();
            _target = ActorOf(Props.Create(() => new FileReader(_testProbe)));
        }

        [TestMethod]
        public void LogsInfoWhenReceivedFile()
        {
            EventFilter.Info($"Processing {NonExistentPath}")
                .ExpectOne(() => _target.Tell(new HashFile(NonExistentPath)));
        }

        [TestMethod]
        public void LogsWarningWhenFileNotFound()
        {
            EventFilter.Warning($"{NonExistentPath}: File not found")
                .ExpectOne(() => _target.Tell(new HashFile(NonExistentPath)));
        }

        [TestMethod]
        public void LogsInfoWhenStartingToReadFile()
        {
            EventFilter.Info($"Reading {_existentPath}").ExpectOne(() => _target.Tell(new HashFile(_existentPath)));
        }

        [TestMethod]
        public void LogsInfoWhenFinishedHashingFile()
        {
            EventFilter.Info($"Finishing {_existentPath}").ExpectOne(() => _target.Tell(new HashFile(_existentPath)));
        }

        [TestMethod]
        public void SendsMessageWithValidFileNameWhenFinishedHashingFile()
        {
            _target.Tell(new HashFile(_existentPath));
            _testProbe.ExpectMsg<FileHashed>(message => message.Path == _existentPath);
        }

    }
}