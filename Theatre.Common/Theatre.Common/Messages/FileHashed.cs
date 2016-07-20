using System;

namespace Theatre.Common.Messages
{
    public class FileHashed
    {
        public string Hash { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}