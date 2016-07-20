using System;

namespace Theatre.Common.Messages
{
    public class FileHashed
    {
        public FileHashed(byte[] hash, DateTime createdDate, DateTime modifiedDate, string path)
        {
            Hash = hash;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            Path = path;
        }

        public byte[] Hash { get; private set; }
        public string Path { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}