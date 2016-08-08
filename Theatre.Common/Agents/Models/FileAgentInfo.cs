namespace Theatre.Common.Agents.Models
{
    #region Usings

    using Akka.Actor;

    #endregion

    public class FileAgentInfo
    {
        public IActorRef Agent { get; set; }

        public string FullPath { get; set; }

        public long Size { get; set; }
    }
}