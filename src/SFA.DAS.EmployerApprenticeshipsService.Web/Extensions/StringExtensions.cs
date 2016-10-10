namespace System
{
    public static class StringExtensions
    {
        public static string FormatPayeForUrl(this string scheme)
        {
            return scheme.Replace("/", "_");
        }

        public static string FormatPayeFromUrl(this string scheme)
        {
            return scheme.Replace("_", "/");
        }
    }
}