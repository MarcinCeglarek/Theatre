using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class DirectoryReader : ReceiveActor
    {
        private readonly IActorRef _directoriesRouter;
        private readonly IActorRef _filesRouter;
        private readonly IActorRef _databaser;
        private readonly IFileSystem _fileSystem;
        private readonly ILoggingAdapter _logger;

        public DirectoryReader(IActorRef directoriesRouter, IActorRef filesRouter, IActorRef databaser, IFileSystem fileSystem)
        {
            _directoriesRouter = directoriesRouter;
            _filesRouter = filesRouter;
            _databaser = databaser;
            _fileSystem = fileSystem;
            _logger = Context.GetLogger();
            ReceiveAsync<HashDirectory>(message => ProcessDirectory(message.FullPath));
        }

        private Task ProcessDirectory(string fullPath)
        {
            this._logger.Info("Processing " + fullPath);
            if (_fileSystem.Directory.Exists(fullPath))
            {
                this._logger.Info("Reading " + fullPath);
                var directories = this._fileSystem.Directory.EnumerateDirectories(fullPath);

                foreach (var directory in directories)
                {
                    _directoriesRouter.Tell(new HashDirectory(directory));
                }

                foreach (var file in _fileSystem.Directory.EnumerateFiles(fullPath))
                {
                    _filesRouter.Tell(new HashFile(file));
                }
                
                _databaser.Tell(new DirectoryHashed(fullPath));
            }
            else
            {
                this._logger.Warning(fullPath + ": Dir not found");
            }

            return Task.FromResult(0);
        }
    }
}