namespace Theatre.Common.Actors
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Theatre.Common.Actors.Models;
    using Theatre.Common.Helpers;
    using Theatre.Common.Messages;
    using Theatre.Common.Messages.Enums;

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

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(1, TimeSpan.Zero,
                e =>
                    {
                        if (e is UnauthorizedAccessException)
                        {
                            this.Logger.Warning(e.Message);
                            var regex = new Regex("Access to the path '([-\\w\\s:`,\\.\\\\]+)' is denied");
                            var result = regex.Match(e.Message);
                            var path = result.Groups[1].Value;
                            var entry = this.Directories.SingleOrDefault(d => d.FullPath == path);
                            if (entry != null)
                            {
                                entry.Message = new DirectoryHashed(path, 0);
                                entry.Status = HashingStatus.PermissionDenied;
                            }
                        }
                        else if (e is System.IO.IOException)
                        {
                            this.Logger.Warning(e.Message);
                            var regex = new Regex("The process cannot access the file '([-\\w\\s:`,\\.\\\\]+)' because it is being used by another process");
                            var result = regex.Match(e.Message);
                            var path = result.Groups[1].Value;
                            var entry = this.Files.SingleOrDefault(d => d.FullPath == path);
                            if (entry != null)
                            {
                                entry.Message = new FileHashed(path, 0, new byte[16], null, null);
                                entry.Status = HashingStatus.PermissionDenied;
                            }
                        }
                        else
                        {
                            this.Logger.Error(e.ToString());
                        }

                        return Directive.Stop;
                    });
        }

        private void ProcessingDirectory()
        {
            // Order of messages here matters - subclasses needs to be put at first, and first matching rule is served
            this.Receive<DirectoryHashed>(message => this.ProcessDirectoryHashed(message));
            this.Receive<FileHashed>(message => this.ProcessFileHashed(message));
            this.Receive<HashDirectory>(message => this.ProcessDirectory(message));
        }

        private void ProcessDirectory(HashDirectory message)
        {
            this.FullPath = message.FullPath;
            this.Logger.Info("Received HashDirectory message for " + this.FullPath);

            if (this.FileSystem.Directory.Exists(this.FullPath))
            {
                this.Logger.Debug("Reading " + this.FullPath);
                var directories = this.FileSystem.Directory.EnumerateDirectories(this.FullPath);

                foreach (var directoryPath in directories)
                {
                    this.Logger.Debug("Creating agent for " + directoryPath);
                    var props = Context.DI().Props<DirectoryReader>();
                    var agent = Context.ActorOf(props, Path.GetFileName(directoryPath).ToActorFriendlyName());
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

        private void ProcessFileHashed(FileHashed message)
        {
            var entry = this.Files.SingleOrDefault(f => f.FullPath == message.Path);
            if (entry == null)
            {
                this.Logger.Error("Received FileHashed message for missing entry " + message.Path);
                return;
            }

            this.Logger.Debug("Reaceived FileHashed message for " + message.Path);
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

            this.Logger.Debug("Received DirectoryHashed message for " + message.FullPath);
            entry.Message = message;
            entry.Status = HashingStatus.Completed;

            this.CheckCurrentDirectoryFinishingCondition();
        }

        private void CheckCurrentDirectoryFinishingCondition()
        {
            if (this.Files.All(f => f.Size.HasValue) && this.Directories.All(d => d.Size.HasValue))
            {
                this.Logger.Info("Finished hashing directory: " + this.FullPath);
                var size = this.Files.Sum(f => f.Size.Value) + this.Directories.Sum(d => d.Size.Value);
                Context.Parent.Tell(new DirectoryHashed(this.FullPath, size));
            }
        }
    }
}