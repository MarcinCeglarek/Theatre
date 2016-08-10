namespace Theatre.Common.Messages.BaseClasses
{
    public abstract class BaseFile
    {
        protected BaseFile(string fullPath)
        {
            this.FullPath = fullPath;
        }

        public string FullPath { get; }
    }
}
