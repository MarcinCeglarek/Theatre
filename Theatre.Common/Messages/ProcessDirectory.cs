namespace Theatre.Common.Messages
{
    #region

    using Theatre.Common.Messages.BaseClasses;

    #endregion

    public class ProcessDirectory : BaseFile
    {
        public ProcessDirectory(string fullPath)
            : base(fullPath)
        {
        }
    }
}