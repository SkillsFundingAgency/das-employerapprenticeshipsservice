using SFA.DAS.EmployerAccounts.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CdnLink(this HtmlHelper html, string folderName, string fileName)
        {
            var cdnLocation = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<Configuration.EmployerAccountsConfiguration>().CdnBaseUrl;

            var trimCharacters = new char[] { '/' };
            return new MvcHtmlString($"{cdnLocation.Trim(trimCharacters)}/{folderName.Trim(trimCharacters)}/{fileName.Trim(trimCharacters)}");
        }

        public static MvcHtmlString CommaSeperatedAddressToHtml(this HtmlHelper htmlHelper, string commaSeperatedAddress)
        {
            var htmlAddress = commaSeperatedAddress.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => $"{line.Trim()}<br/>")
                .Aggregate("", (x, y) => x + y);

            return new MvcHtmlString(htmlAddress);
        }

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
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            return configuration.ZenDeskSnippetKey;
        }

        public static string GetZenDeskSnippetSectionId(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            return configuration.ZenDeskSectionId;
        }
    }
        
}