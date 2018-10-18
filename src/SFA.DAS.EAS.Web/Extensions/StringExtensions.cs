using System.Net;

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

        public static string HtmlDecode(this string input)
        {
            var output = WebUtility.HtmlDecode(input);
            return output;
        }

        public static string HtmlEncode(this string input)
        {
            var output = WebUtility.HtmlEncode(input);
            return output;
        }
    }
}