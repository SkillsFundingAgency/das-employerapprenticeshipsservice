using System.Net;

namespace SFA.DAS.EAS.Portal.Extensions
{
    public static class StringExtensions
    {
        public static string HtmlDecode(this string input)
        {
            return WebUtility.HtmlDecode(input);
        }
    }
}