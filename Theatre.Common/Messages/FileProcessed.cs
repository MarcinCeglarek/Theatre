namespace Theatre.Common.Messages
{
    #region

    using System;

    using Theatre.Common.Messages.BaseClasses;

    #endregion

    public class FileProcessed : BaseFile
    {
        public FileProcessed(string fullPath, long size, byte[] hash, DateTime? createdDate, DateTime? modifiedDate)
            : base(fullPath)
        {
            this.Size = size;
            this.Hash = hash;
            this.CreatedDate = createdDate;
            this.LastWriteDate = modifiedDate;
        }

        public DateTime? CreatedDate { get; }

        public byte[] Hash { get; }

        public DateTime? LastWriteDate { get; }

        public long Size { get; }
    }
}