using Theatre.Common.Messages;

namespace Theatre.Common.Agents.Models
{
    #region Usings

    using System.IO;

    using Theatre.Common.Messages.Enums;

    #endregion

    public class DirectoryAgentInfo : FileAgentInfo
    {
        public string Name => Path.GetDirectoryName(this.FullPath);

        public new DirectoryHashed Message { get; set; }

        public new long? Size => this.Message?.Size;

        public HashingStatus Status { get; set; }
    }
}