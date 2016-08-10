namespace Theatre.Common.Actors.Models
{
    #region Usings

    using System.IO;

    using Theatre.Common.Messages;
    using Theatre.Common.Messages.Enums;

    #endregion

    public class DirectoryAgentInfo : FileAgentInfo
    {
        public string Name => Path.GetDirectoryName(this.FullPath);

        public new DirectoryProcessed Message { get; set; }

        public new long? Size => this.Message?.Size;
    }
}