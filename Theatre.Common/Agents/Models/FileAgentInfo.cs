using Theatre.Common.Messages;

namespace Theatre.Common.Agents.Models
{
    #region Usings

    using Akka.Actor;

    using Theatre.Common.Messages.Enums;

    #endregion

    public class FileAgentInfo
    {
        public IActorRef Agent { get; set; }

        public string FullPath { get; set; }

        public long? Size => this.Message?.Size;

        public FileHashed Message {get;set;}

        public HashingStatus Status { get; set; }
    }
}