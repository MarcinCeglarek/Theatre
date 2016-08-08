namespace Theatre.Common.Tests.Agents
{
    #region Usings

    using System.Collections.Generic;
    using System.IO.Abstractions;

    using Akka.Actor;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;
    using Akka.TestKit;
    using Akka.TestKit.Xunit2;

    using Autofac;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Theatre.Common.Agents;
    using Theatre.Common.Messages;

    using Xunit;

    #endregion

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

        public DirectoryReaderTests()
            : base(@"akka.loglevel = DEBUG")
        {
        }

        [Fact]
        public void StartsProcessingDirectory()
        {
            this.EventFilter.Info("Received HashDirectory message for " + NonExistingDirPath)
                .ExpectOne(() => this.GetTargetAgent(NonExistingDirPath));
        }

        [Fact]
        public void LogsInfoWhenStartingProcessingDirectory()
        {
            this.EventFilter.Debug("Reading " + ExistingDirPath).ExpectOne(() => this.GetTargetAgent(ExistingDirPath));
        }

        [Fact]
        public void AgentLogsErrorIfDirectoryDoesNotExists()
        {
            this.EventFilter.Error(NonExistingDirPath + ": Dir not found")
                .ExpectOne(() => this.GetTargetAgent(NonExistingDirPath));
        }

        [Fact]
        public void AgentDiesIfDirectoryDoesNotExists()
        {
            var target = this.GetTargetAgent(NonExistingDirPath);
            this.Watch(target);
            this.ExpectTerminated(target);
        }

        [Fact]
        public void CreatesAgentForEachSubdirectory()
        {
            this.EventFilter.Info(null, "Received HashDirectory message for ")
                .Expect(3, () => this.GetTargetAgent(ExistingDirPath));
        }

        private IActorRef GetTargetAgent(string path)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => this.GetMockFileSystem()).As<IFileSystem>();

            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var propsResolver = new AutoFacDependencyResolver(container, this.Sys);
                var props = this.Sys.DI().Props<DirectoryReader>();
                var target = this.Sys.ActorOf(props);
                target.Tell(new HashDirectory(path));
                return target;
            }
        }

        private IFileSystem GetMockFileSystem()
        {
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.Directory.Exists(ExistingDirPath)).Returns(true);
            fileSystem.Setup(f => f.Directory.Exists(NonExistingDirPath)).Returns(false);
            fileSystem.Setup(f => f.Directory.EnumerateDirectories(ExistingDirPath)).Returns(this.directories);
            fileSystem.Setup(f => f.Directory.EnumerateFiles(ExistingDirPath)).Returns(this.files);

            return fileSystem.Object;
        }
    }
}