#region Usings

using System.Collections.Generic;
using System.IO.Abstractions;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Theatre.Common.Agents;
using Theatre.Common.Messages;
using Xunit;

#endregion

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class DirectoryReaderTests : TestKit
    {
        private const string ExistingDirPath = "Existing/Dir";
        private const string NonExistingDirPath = "Non/Existent/Path";

        private readonly IReadOnlyCollection<string> directories = new List<string>
        {
            ExistingDirPath + "/d1",
            ExistingDirPath + "/d2"
        };

        private readonly IReadOnlyCollection<string> files = new List<string>
        {
            ExistingDirPath + "/f1.abc",
            ExistingDirPath + "/f2.def",
            ExistingDirPath + "/f3.ghj"
        };

        private TestProbe databaser;

        public DirectoryReaderTests()
            : base(@"akka.loglevel = DEBUG")
        {
            databaser = CreateTestProbe();

            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().As<IFileSystem>();

            var container = builder.Build();
        }

        [Fact]
        public void StartsProcessingDirectory()
        {
            EventFilter.Info("Processing " + NonExistingDirPath)
                .ExpectOne(() => GetTargetAgent(NonExistingDirPath));
        }

        [Fact]
        public void LogsInfoWhenStartingProcessingDirectory()
        {
            EventFilter.Debug("Reading " + ExistingDirPath)
                .ExpectOne(() => GetTargetAgent(ExistingDirPath));
        }

        [Fact]
        public void AgentLogsErrorIfDirectoryDoesNotExists()
        {
            EventFilter.Error(NonExistingDirPath + ": Dir not found")
                .ExpectOne(() => GetTargetAgent(NonExistingDirPath));
        }

        [Fact]
        public void AgentDiesIfDirectoryDoesNotExists()
        {
            var target = GetTargetAgent(NonExistingDirPath);
            Watch(target);
            ExpectTerminated(target);
        }

        [Fact]
        public void CreatesAgentForEachSubdirectory()
        {
            EventFilter.Info()
                .Expect(3, () => GetTargetAgent(ExistingDirPath));
        }

        private TestActorRef<DirectoryReader> GetTargetAgent(string path)
        {
            var filesystem = GetMockFileSystem();
            var target = ActorOfAsTestActorRef<DirectoryReader>(
                Props.Create(
                    () =>
                        new DirectoryReader(
                            filesystem.Object
                            )));
            target.Tell(new HashDirectory(path));
            return target;
        }

        private Mock<IFileSystem> GetMockFileSystem()
        {
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.Directory.Exists(ExistingDirPath)).Returns(true);
            fileSystem.Setup(f => f.Directory.Exists(NonExistingDirPath)).Returns(false);
            fileSystem.Setup(f => f.Directory.EnumerateDirectories(ExistingDirPath)).Returns(directories);
            fileSystem.Setup(f => f.Directory.EnumerateFiles(ExistingDirPath)).Returns(files);

            return fileSystem;
        }
    }
}