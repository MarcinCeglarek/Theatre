namespace Theatre.Common.Messages
{
    #region

    using Theatre.Common.Messages.BaseClasses;

    #endregion

    public class DirectoryProcessed : BaseFile
    {
        public DirectoryProcessed(string fullPath, long size)
            : base(fullPath)
        {
            this.Size = size;
        }

        public long Size { get; }
    }
}