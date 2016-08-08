#region Usings

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Theatre.Common.Agents.Models;
using Theatre.Common.Messages;

#endregion

namespace Theatre.Common.Agents
{
    public class DirectoryReader : ReceiveActor
    {
        public DirectoryReader(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            Logger = Context.GetLogger();
            ReceiveAsync<HashDirectory>(message => ProcessDirectory(message.FullPath));

            Directories = new List<DirectoryAgentInfo>();
            Files = new List<FileAgentInfo>();
        }

        private IFileSystem FileSystem { get; }
        private ILoggingAdapter Logger { get; }
        private IList<DirectoryAgentInfo> Directories { get; }
        private IList<FileAgentInfo> Files { get; }

        private Task ProcessDirectory(string fullPath)
        {
            Logger.Info("Processing " + fullPath);
            if (FileSystem.Directory.Exists(fullPath))
            {
                Logger.Debug("Reading " + fullPath);
                var directories = FileSystem.Directory.EnumerateDirectories(fullPath);

                foreach (var directoryPath in directories)
                {
                    var props = Context.DI().Props<DirectoryReader>();
                    var directoryAgent = Context.ActorOf(props, "NameOfDirectoryReader");
                    directoryAgent.Tell(new HashDirectory(directoryPath));

                    var dirInfo = new DirectoryAgentInfo
                    {
                        Agent = directoryAgent,
                        FullPath = Path.Combine(fullPath, directoryPath),
                        Size = 0
                    };

                    Directories.Add(dirInfo);
                }

                foreach (var file in FileSystem.Directory.EnumerateFiles(fullPath))
                {
                    /*this.filesRouter.Tell(new HashFile(file));*/
                }
            }
            else
            {
                Logger.Error(fullPath + ": Dir not found");
                Self.Tell(PoisonPill.Instance);
            }

            return Task.FromResult(0);
        }

        private Task ProcessDirectoryHashed(DirectoryHashed message)
        {
            /*this.databaser.Tell(new DirectoryHashed(fullPath));*/
            return Task.FromResult(0);
        }
    }
}