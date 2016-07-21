using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
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
    public class DirectoryReaderTests : TestKit
    {
        private const string NonExistingDirPath = "Non/Existent/Path";
        private const string ExistingDirPath = "Existing/Dir";

        private IActorRef _target;
        private TestProbe _databaser;
        private TestProbe _directoriesRouter;
        private TestProbe _filesRouter;

        private List<string> _directories = new List<string>()
        {
            ExistingDirPath + "/d1",
            ExistingDirPath + "/d2"
        };

        private List<string> _files = new List<string>()
        {
            ExistingDirPath + "/f1.abc",
            ExistingDirPath + "/f2.def",
            ExistingDirPath + "/f3.ghj"
        };


        [TestInitialize]
        public void BeforeTest()
        {
            _databaser = this.CreateTestProbe();
            _directoriesRouter = this.CreateTestProbe();
            _filesRouter = this.CreateTestProbe();
            var filesystem = CreateMockFileSystem();
            _target = ActorOf(Props.Create(() => new DirectoryReader(_directoriesRouter, _filesRouter, _databaser, filesystem.Object)));
        }

        private Mock<IFileSystem> CreateMockFileSystem()
        {
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.Directory.Exists(ExistingDirPath)).Returns(true);
            fileSystem.Setup(f => f.Directory.Exists(NonExistingDirPath)).Returns(false);

            fileSystem.Setup(f => f.Directory.EnumerateDirectories(ExistingDirPath)).Returns(new List<string>()
            );

            fileSystem.Setup(f => f.Directory.EnumerateFiles(ExistingDirPath)).Returns(new List<string>()
           );

            return fileSystem;
        }

        [TestMethod]
        public void StartsProcessingDirectory()
        {
            EventFilter.Info("Processing " + NonExistingDirPath)
                .ExpectOne(() => _target.Tell(new HashDirectory(NonExistingDirPath)));
        }

        [TestMethod]
        public void LogsWarningIfDirectoryDoesNotExists()
        {
            EventFilter.Warning(NonExistingDirPath + ": Dir not found")
                .ExpectOne(() => _target.Tell(new HashDirectory(NonExistingDirPath)));
        }

        [TestMethod]
        public void LogsInfoWhenStartingProcessingDirectory()
        {
            EventFilter.Info("Reading " + ExistingDirPath).ExpectOne(() => _target.Tell(new HashDirectory(ExistingDirPath)));
        }

        [TestMethod]
        public void SendsMessageWhenFinishedReadingDirectory()
        {
            _target.Tell(new HashDirectory(ExistingDirPath));
            _databaser.ExpectMsg<DirectoryHashed>(message => message.FullPath == ExistingDirPath);
        }

        [TestMethod]
        public void SendsMessageForEachFoundDirectory()
        {
            _target.Tell(new HashDirectory(ExistingDirPath));
            for (var i = 0; i < _directories.Count; i++)
            {
                _filesRouter.ExpectMsg<HashFile>();
            }
        }

        [TestMethod]
        public void SendsMessageWithCorrectPathsForEachFoundDirectory()
        {
            _target.Tell(new HashDirectory(ExistingDirPath));
            var m1 = _directoriesRouter.ExpectMsg<HashDirectory>();
            Assert.IsTrue(m1.FullPath == ExistingDirPath + "/1" || m1.FullPath == ExistingDirPath + "/2");
            var m2 = _directoriesRouter.ExpectMsg<HashDirectory>();
            Assert.IsTrue(m2.FullPath == ExistingDirPath + "/1" || m2.FullPath == ExistingDirPath + "/2");
            Assert.AreNotEqual(m1.FullPath, m2.FullPath);
        }

        [TestMethod]
        public void SendsMessageForEachFoundFile()
        {
            _target.Tell(new HashDirectory(ExistingDirPath));
            for (var i = 0; i < _files.Count; i++)
            {
                _filesRouter.ExpectMsg<HashFile>();
            }
        }

        [TestMethod]
        public void SendsMessageWithCorrectPathsForEachFoundFile()
        {
            _target.Tell(new HashDirectory(ExistingDirPath));
            var m1 = _filesRouter.ExpectMsg<HashFile>();

            var m2 = _filesRouter.ExpectMsg<HashFile>();
            var m3 = _filesRouter.ExpectMsg<HashFile>();
        }
    }
}