using SFA.DAS.EmployerFinance.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {
            var keywords = string.Join(",", labels
               .Where(label => !string.IsNullOrEmpty(label))
               .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                + "] });</script>";

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