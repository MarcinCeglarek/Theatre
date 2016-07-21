using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
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
        private const string NotExistingFilePath = "Non/Existent/Path";

        private const string ExistingFilePath = "Existent/Path";

        private readonly DateTime _createdDate = DateTime.Now.AddDays(-2);

        private readonly byte[] _existingFileContent = {0, 1, 2, 3, 4, 5, 6, 7, 9, 10};

        private readonly DateTime _writeDate = DateTime.Now.AddDays(-1);

        private IActorRef _target;

        private TestProbe _testProbe;

        [TestInitialize]
        public void BeforeTest()
        {
            _testProbe = CreateTestProbe();

            var fileSystemMock = CreateFileSystemMock();
            _target = ActorOf(Props.Create(() => new FileReader(_testProbe, fileSystemMock.Object)));
        }

        private Mock<IFileSystem> CreateFileSystemMock()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(f => f.File.Exists(ExistingFilePath)).Returns(true);
            fileSystemMock.Setup(f => f.File.Exists(NotExistingFilePath)).Returns(false);
            fileSystemMock.Setup(f => f.File.ReadAllBytes(NotExistingFilePath)).Throws(new FileNotFoundException());
            fileSystemMock.Setup(f => f.File.ReadAllBytes(ExistingFilePath)).Returns(_existingFileContent);
            fileSystemMock.Setup(f => f.File.GetCreationTimeUtc(ExistingFilePath)).Returns(_createdDate);
            fileSystemMock.Setup(f => f.File.GetCreationTimeUtc(NotExistingFilePath))
                .Throws(new FileNotFoundException());
            fileSystemMock.Setup(f => f.File.GetLastWriteTimeUtc(ExistingFilePath)).Returns(_writeDate);
            fileSystemMock.Setup(f => f.File.GetLastWriteTimeUtc(NotExistingFilePath))
                .Throws(new FileNotFoundException());
            return fileSystemMock;
        }

        [TestMethod]
        public void LogsInfoWhenReceivedFile()
        {
            EventFilter.Info($"Processing {NotExistingFilePath}")
                .ExpectOne(() => _target.Tell(new HashFile(NotExistingFilePath)));
        }

        [TestMethod]
        public void LogsWarningWhenFileNotFound()
        {
            EventFilter.Warning($"{NotExistingFilePath}: File not found")
                .ExpectOne(() => _target.Tell(new HashFile(NotExistingFilePath)));
        }

        [TestMethod]
        public void LogsInfoWhenStartingToReadFile()
        {
            EventFilter.Info($"Reading {ExistingFilePath}")
                .ExpectOne(() => _target.Tell(new HashFile(ExistingFilePath)));
        }

        [TestMethod]
        public void LogsInfoWhenFinishedHashingFile()
        {
            EventFilter.Info($"Finishing {ExistingFilePath}")
                .ExpectOne(() => _target.Tell(new HashFile(ExistingFilePath)));
        }

        [TestMethod]
        public void SendsMessageWithValidFileNameWhenFinishedHashingFile()
        {
            _target.Tell(new HashFile(ExistingFilePath));
            var message = _testProbe.ExpectMsg<FileHashed>();
            Assert.AreEqual(message.Path, ExistingFilePath);
        }

        [TestMethod]
        public void ChecksCreationDatesOnSentMessage()
        {
            _target.Tell(new HashFile(ExistingFilePath));
            var message = _testProbe.ExpectMsg<FileHashed>();
            Assert.AreEqual(message.CreatedDate, _createdDate);
        }

        [TestMethod]
        public void ChecksLastWriteDatesOnSentMessage()
        {
            _target.Tell(new HashFile(ExistingFilePath));
            var message = _testProbe.ExpectMsg<FileHashed>();
            Assert.AreEqual(message.LastWriteDate, _writeDate);
        }

        [TestMethod]
        public void ChecksSizeOnSentMessage()
        {
            _target.Tell(new HashFile(ExistingFilePath));
            var message = _testProbe.ExpectMsg<FileHashed>();
            Assert.AreEqual(message.Size, _existingFileContent.Length);
        }

        [TestMethod]
        public void ChecksHashOnSentMessage()
        {
            _target.Tell(new HashFile(ExistingFilePath));
            var hash = MD5.Create().ComputeHash(_existingFileContent);
            var message = _testProbe.ExpectMsg<FileHashed>();
            Assert.IsTrue(hash.SequenceEqual(message.Hash));
        }
    }
}