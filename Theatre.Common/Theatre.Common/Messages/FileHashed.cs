using System;

namespace Theatre.Common.Messages
{
    public class FileHashed
    {
        public FileHashed(string path, long size, byte[] hash, DateTime createdDate, DateTime modifiedDate)
        {
            Path = path;
            Size = size;
            Hash = hash;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }

        public byte[] Hash { get; private set; }
        public string Path { get; private set; }
        public long Size { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}