using SFA.DAS.EmployerFinance.Configuration;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {
            var apiCallString =
                "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    if (!first) apiCallString += ",";
                    first = false;

                    apiCallString += $"'{ EscapeApostrophes(label) }'";
                }
            }

            apiCallString += "] });</script>";

            return MvcHtmlString.Create(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }

        public static string GetZenDeskSnippetKey(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();            
            return configuration.ZenDeskSnippetKey;
        }

        public static string GetZenDeskSnippetSectionId(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            return configuration.ZenDeskSectionId;
        }
    }
}   