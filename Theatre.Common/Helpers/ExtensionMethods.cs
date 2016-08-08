namespace Theatre.Common.Helpers
{
    public static class ExtensionMethods
    {
        public static string ToActorFriendlyName(this string path)
        {
            return path.Replace('\\', '!').Replace(' ', '*').Replace('{', '(').Replace('[', '(').Replace(']', ')').Replace('}', ')');
        }
    }
}