namespace Theatre.Common.Messages
{
    #region

    using System.Collections.Generic;

    using Theatre.Common.Messages.BaseClasses;
    using Theatre.Common.Messages.Enums;

    #endregion

    public class DetailResponse : BaseFile
    {
        public DetailResponse(string fullPath, List<FileTypeHistogram> fileTypeHistograms)
            : base(fullPath)
        {
            this.FileTypeHistograms = fileTypeHistograms;
        }

        public List<FileTypeHistogram> FileTypeHistograms { get; }

        public long Size { get; set; }

        public class FileTypeHistogram
        {
            public FileType FileType { get; set; }

            public double Percent { get; set; }
        }
    }
}