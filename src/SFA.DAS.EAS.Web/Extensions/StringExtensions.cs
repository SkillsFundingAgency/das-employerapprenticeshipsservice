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

        public static bool ParseHideNavFromViewBag(dynamic viewBag)
        {
            if (viewBag.HideNav == null)
            {
                return false;
            }

            if (viewBag.HideNav is string)
            {
                return !string.IsNullOrWhiteSpace(viewBag.HideNav);
            }

            if (viewBag.HideNav is bool)
            {
                return viewBag.HideNav;
            }

            return false;
        }
    }
}