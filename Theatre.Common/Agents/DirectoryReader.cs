namespace Theatre.Common.Agents
{
    #region Usings

    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Theatre.Common.Agents.Models;
    using Theatre.Common.Messages;

    #endregion

    public class DirectoryReader : ReceiveActor
    {
        public DirectoryReader(IFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
            this.Logger = Context.GetLogger();

            this.Directories = new List<DirectoryAgentInfo>();
            this.Files = new List<FileAgentInfo>();
            this.Become(this.ProcessingDirectory);
        }

        private IFileSystem FileSystem { get; }

        private ILoggingAdapter Logger { get; }

        private string FullPath { get; set; }

        private IList<DirectoryAgentInfo> Directories { get; }

        private IList<FileAgentInfo> Files { get; }

        public void ProcessingDirectory()
        {
            // Order of messages here matters - subclasses needs to be put at first, and first matching rule is served
            this.Receive<DirectoryHashed>(message => this.ProcessDirectoryHashed(message));
            this.Receive<FileHashed>(message => this.FileHashedHandler(message));
            this.Receive<HashDirectory>(message => this.ProcessDirectory(message));
        }

        private void ProcessDirectory(HashDirectory message)
        {
            this.FullPath = message.FullPath;
            this.Logger.Error("Rrecieved HashDirectory message for " + this.FullPath);
            if (this.FileSystem.Directory.Exists(this.FullPath))
            {
                this.Logger.Debug("Reading " + this.FullPath);
                var directories = this.FileSystem.Directory.EnumerateDirectories(this.FullPath);

                foreach (var directoryPath in directories)
                {
                    this.Logger.Debug("Creating agent for " + directoryPath);
                    var props = Context.DI().Props<DirectoryReader>();
                    var agent = Context.ActorOf(props);
                    agent.Tell(new HashDirectory(directoryPath));

                    var dirInfo = new DirectoryAgentInfo
                                      {
                                          Agent = agent, 
                                          FullPath = Path.Combine(this.FullPath, directoryPath)
                                      };

                    this.Directories.Add(dirInfo);
                }

                foreach (var file in this.FileSystem.Directory.EnumerateFiles(this.FullPath))
                {
                    var props = Context.DI().Props<FileReader>();
                    var agent = Context.ActorOf(props);
                    agent.Tell(new HashFile(file));

                    var fileInfo = new FileAgentInfo { Agent = agent, FullPath = file };

                    this.Files.Add(fileInfo);
                }
            }
            else
            {
                this.Logger.Error(this.FullPath + ": Dir not found");
                this.Self.Tell(PoisonPill.Instance);
            }
        }

        private void FileHashedHandler(FileHashed message)
        {
            var entry = this.Files.SingleOrDefault(f => f.FullPath == message.Path);
            if (entry == null)
            {
                this.Logger.Error("Received FileHashed message for missing entry " + message.Path);
                return;
            }

            this.Logger.Warning("Reaceived FileHashed message for " + message.Path);
            entry.Message = message;

            this.CheckCurrentDirectoryFinishingCondition();
        }

        private void ProcessDirectoryHashed(DirectoryHashed message)
        {
            var entry = this.Directories.SingleOrDefault(d => d.FullPath == message.FullPath);
            if (entry == null)
            {
                this.Logger.Error("Received DirectoryHashed message for missing entry " + message.FullPath);
                return;
            }

            this.Logger.Warning("Received DirectoryHashed message for " + message.FullPath);
            entry.Message = message;

            this.CheckCurrentDirectoryFinishingCondition();
        }

        private void CheckCurrentDirectoryFinishingCondition()
        {
            if (this.Files.All(f => f.Size.HasValue) && this.Directories.All(d => d.Size.HasValue))
            {
                this.Logger.Error("Finished hashing directory: " + this.FullPath);
                var size = this.Files.Sum(f => f.Size.Value) + this.Directories.Sum(d => d.Size.Value);
                Context.Parent.Tell(new DirectoryHashed(this.FullPath, size));
            }
        }
    }
}