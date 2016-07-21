namespace Theatre.Common.Messages
{
    public class HashFile
    {
        public HashFile(string fullPath)
        {
            this.FullPath = fullPath;
        }

        public string FullPath { get; private set; }
    }
}