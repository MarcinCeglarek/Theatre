namespace Theatre.Common.Actors
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

            this.ReceiveAsync<FileProcessed>(this.ProcessFileHashedMessage);
        }

        private Task ProcessFileHashedMessage(FileProcessed message)
        {
            this.logger.Info($"{message.FullPath} hashed");
            this.logger.Info($"{message.FullPath} size: {message.Size}");
            return Task.FromResult(0);
        }
    }
}