namespace Theatre.Common.Helpers
{
    using System.Linq;

    using Akka.Util.Internal;

    public static class ExtensionMethods
    {
        public static string ToActorFriendlyName(this string path)
        {
            var encodedPathParts = path.Split('\\').Select(EncodeString);
            return string.Join("\\", encodedPathParts);
        }

        private static string EncodeString(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}