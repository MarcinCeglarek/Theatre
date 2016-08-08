namespace Theatre.Common.Messages
{
    public class DirectoryHashed : HashDirectory
    {
        public DirectoryHashed(string fullPath, long directorySize)
            : base(fullPath)
        {
            this.DirectorySize = directorySize;
        }

        public long DirectorySize { get; }
    }
}