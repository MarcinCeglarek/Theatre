using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class DirectoryReader : ReceiveActor
    {
        private readonly IActorRef _databaser;
        private readonly ILoggingAdapter _logger;

        public DirectoryReader(IActorRef databaser)
        {
            _databaser = databaser;
            _logger = Context.GetLogger();
            ReceiveAsync<HashDirectory>(message => ProcessDirectory(message.FullPath));
        }

        private Task ProcessDirectory(string fullPath)
        {
            this._logger.Info("Processing " + fullPath);
            if (Directory.Exists(fullPath))
            {
                this._logger.Info("Reading " + fullPath);

                ProcessChildrenDirectories();
                ProcessChildrenFiles();

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