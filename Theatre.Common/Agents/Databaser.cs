namespace Theatre.Common.Agents
{
    #region Usings

    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Theatre.Common.Messages;

    #endregion

    public class Databaser : ReceiveActor
    {
        private readonly ILoggingAdapter logger;

        public Databaser()
        {
            this.logger = Context.GetLogger();

            this.ReceiveAsync<FileHashed>(this.ProcessFileHashedMessage);
        }

        private Task ProcessFileHashedMessage(FileHashed message)
        {
            this.logger.Info($"{message.Path} hashed");
            this.logger.Info($"{message.Path} size: {message.Size}");
            return Task.FromResult(0);
        }
    }
}