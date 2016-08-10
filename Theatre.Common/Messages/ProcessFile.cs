namespace Theatre.Common.Messages
{
    #region

    using Theatre.Common.Messages.BaseClasses;

    #endregion

    public class ProcessFile : BaseFile
    {
        public ProcessFile(string fullPath)
            : base(fullPath)
        {
        }
    }
}