﻿namespace Theatre.Common.Tests.Actors
{
    #region Usings

    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Security.Cryptography;

    using Akka.Actor;
    using Akka.TestKit;
    using Akka.TestKit.Xunit2;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Theatre.Common.Actors;
    using Theatre.Common.Messages;

    using Xunit;

    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    #endregion

    [TestClass]
    public class FileReaderTests : TestKit
    {
        private const string ExistingFilePath = "Existent/Path";

        private const string NotExistingFilePath = "Non/Existent/Path";

        private readonly DateTime createdDate = DateTime.Now.AddDays(-2);

        private readonly byte[] existingFileContent = { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10 };

        private readonly DateTime writeDate = DateTime.Now.AddDays(-1);

        private TestActorRef<FileReader> Target { get; set; }

        public FileReaderTests()
            : base(@"akka.loglevel = DEBUG")
        {
            var fileSystemMock = this.CreateMockFileSystem();
            this.Target = this.ActorOfAsTestActorRef<FileReader>(Props.Create(() => new FileReader(fileSystemMock.Object)));
        }

        [Fact]
        public void ChecksCreationDatesOnSentMessage()
        {
            this.Target.Tell(new ProcessFile(ExistingFilePath));
            var message = this.ExpectMsg<FileProcessed>();
            Assert.AreEqual(message.CreatedDate, this.createdDate);
        }

        [Fact]
        public void ChecksHashOnSentMessage()
        {
            this.Target.Tell(new ProcessFile(ExistingFilePath));
            var hash = MD5.Create().ComputeHash(this.existingFileContent);
            var message = this.ExpectMsg<FileProcessed>();
            Assert.IsTrue(hash.SequenceEqual(message.Hash));
        }

        [Fact]
        public void ChecksLastWriteDatesOnSentMessage()
        {
            this.Target.Tell(new ProcessFile(ExistingFilePath));
            var message = this.ExpectMsg<FileProcessed>();
            Assert.AreEqual(message.LastWriteDate, this.writeDate);
        }

        [Fact]
        public void ChecksSizeOnSentMessage()
        {
            this.Target.Tell(new ProcessFile(ExistingFilePath));
            var message = this.ExpectMsg<FileProcessed>();
            Assert.AreEqual(message.Size, this.existingFileContent.Length);
        }

        [Fact]
        public void LogsInfoWhenFinishedHashingFile()
        {
            this.EventFilter.Info($"Finishing {ExistingFilePath}")
                .ExpectOne(() => this.Target.Tell(new ProcessFile(ExistingFilePath)));
        }

        [Fact]
        public void LogsInfoWhenReceivedFile()
        {
            this.EventFilter.Info($"Processing {NotExistingFilePath}")
                .ExpectOne(() => this.Target.Tell(new ProcessFile(NotExistingFilePath)));
        }

        [Fact]
        public void LogsInfoWhenStartingToReadFile()
        {
            this.EventFilter.Debug($"Reading {ExistingFilePath}")
                .ExpectOne(() => this.Target.Tell(new ProcessFile(ExistingFilePath)));
        }

        [Fact]
        public void LogsWarningWhenFileNotFound()
        {
            this.EventFilter.Warning($"{NotExistingFilePath}: File not found")
                .ExpectOne(() => this.Target.Tell(new ProcessFile(NotExistingFilePath)));
        }

        [Fact]
        public void SendsMessageWithValidFileNameWhenFinishedHashingFile()
        {
            this.Target.Tell(new ProcessFile(ExistingFilePath));
            var message = this.ExpectMsg<FileProcessed>();
            Assert.AreEqual(message.FullPath, ExistingFilePath);
        }

        private Mock<IFileSystem> CreateMockFileSystem()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(f => f.File.Exists(ExistingFilePath)).Returns(true);
            fileSystemMock.Setup(f => f.File.Exists(NotExistingFilePath)).Returns(false);
            fileSystemMock.Setup(f => f.File.ReadAllBytes(NotExistingFilePath)).Throws(new FileNotFoundException());
            fileSystemMock.Setup(f => f.File.ReadAllBytes(ExistingFilePath)).Returns(this.existingFileContent);
            fileSystemMock.Setup(f => f.File.GetCreationTimeUtc(ExistingFilePath)).Returns(this.createdDate);
            fileSystemMock.Setup(f => f.File.GetLastWriteTimeUtc(ExistingFilePath)).Returns(this.writeDate);

            return fileSystemMock;
        }
    }
}