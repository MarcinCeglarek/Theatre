using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class Databaser : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;

        public Databaser()
        {
            this._logger = Context.GetLogger();

            ReceiveAsync<FileHashed>(ProcessFileHashedMessage);
        }

        private Task ProcessFileHashedMessage(FileHashed message)
        {
            _logger.Info($"{message.Path} hashed");
            _logger.Info($"{message.Path} size: {message.Size}");
            return Task.FromResult(0);
        }
    }
}