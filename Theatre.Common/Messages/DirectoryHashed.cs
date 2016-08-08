namespace Theatre.Common.Messages
{
    public class DirectoryHashed : HashDirectory
    {
        public DirectoryHashed(string fullPath, long size)
            : base(fullPath)
        {
            this.Size = size;
        }

        public long Size { get; }
    }
}