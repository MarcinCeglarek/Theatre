namespace Theatre.Common.Agents.Models
{
    #region Usings

    using System.IO;

    #endregion

    public class DirectoryAgentInfo : FileAgentInfo
    {
        public string Name => Path.GetDirectoryName(this.FullPath);
    }
}