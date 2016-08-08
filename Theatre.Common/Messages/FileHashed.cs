namespace Theatre.Common.Messages
{
    #region Usings

    using System;

    #endregion

    public class FileHashed
    {
        public FileHashed(string path, long size, byte[] hash, DateTime createdDate, DateTime modifiedDate)
        {
            this.Path = path;
            this.Size = size;
            this.Hash = hash;
            this.CreatedDate = createdDate;
            this.LastWriteDate = modifiedDate;
        }

        public DateTime CreatedDate { get; }

        public byte[] Hash { get; }

        public DateTime LastWriteDate { get; }

        public string Path { get; }

        public long Size { get; }
    }
}