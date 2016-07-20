using System;

namespace Theatre.Common.Messages
{
    public class FileHashed
    {
        public FileHashed(string hash, DateTime createdDate, DateTime modifiedDate, string path, string filename)
        {
            Hash = hash;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            Path = path;
        }

        public string Hash { get; private set; }
        public string Path { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}