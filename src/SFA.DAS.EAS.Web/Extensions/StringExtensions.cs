namespace SFA.DAS.EAS.Web.Extensions
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

        public static decimal? AsNullableDecimal(this string input)
        {
            var result = default(decimal?);
            decimal parsed;
            if (decimal.TryParse(input, out parsed))
            {
                result = parsed;
            }

            return result;
        }
    }
}