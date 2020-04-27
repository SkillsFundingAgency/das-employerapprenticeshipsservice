using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Models.Links;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

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
                .Aggregate(string.Empty, (x, y) => x + y);

            return new MvcHtmlString(htmlAddress);
        }

        public static bool IsSupportUser(this HtmlHelper htmlHelper)
        {
            if (!(htmlHelper.ViewContext.Controller.ControllerContext.HttpContext.User.Identity is ClaimsIdentity claimsIdentity) || !claimsIdentity.IsAuthenticated)
            {
                return false;
            }

            return claimsIdentity.Claims.Any(c => c.Type == claimsIdentity.RoleClaimType && c.Value.Equals(ControllerConstants.Tier2UserClaim));
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
        public static string GetZenDeskCobrowsingSnippetKey(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();
            return configuration.ZenDeskCobrowsingSnippetKey;
        }

        public static IHeaderViewModel GetHeaderViewModel(this HtmlHelper html, bool useLegacyStyles = false)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();

            var headerModel = new HeaderViewModel(new HeaderConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl,
                ApplicationBaseUrl = configuration.EmployerAccountsBaseUrl,
                EmployerCommitmentsBaseUrl = configuration.EmployerCommitmentsBaseUrl,
                EmployerFinanceBaseUrl = configuration.EmployerFinanceBaseUrl,
                AuthenticationAuthorityUrl = configuration.Identity.BaseAddress,
                ClientId = configuration.Identity.ClientId,
                EmployerRecruitBaseUrl = configuration.EmployerRecruitBaseUrl,
                SignOutUrl = new Uri($"{configuration.EmployerAccountsBaseUrl}/service/signOut"),
                ChangeEmailReturnUrl = new System.Uri(configuration.EmployerAccountsBaseUrl + "/service/email/change"),
                ChangePasswordReturnUrl = new System.Uri(configuration.EmployerAccountsBaseUrl + "/service/password/change")
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["HashedAccountId"]?.ToString()
            },
            useLegacyStyles: useLegacyStyles
            );

            headerModel.SelectMenu(html.ViewContext.RouteData.Values["Controller"].ToString() == "EmployerCommitments" ? "EmployerCommitments" : html.ViewBag.Section);

            if (html.ViewBag.HideNav != null && html.ViewBag.HideNav)
            {
                headerModel.HideMenu();
            }

            if (html.ViewData.Model?.GetType().GetProperty("HideHeaderSignInLink") != null)
            {
                headerModel.RemoveLink<SignIn>();
            }

            return headerModel;
        }

        public static IFooterViewModel GetFooterViewModel(this HtmlHelper html, bool useLegacyStyles = false)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerAccountsConfiguration>();

            return new FooterViewModel(new FooterConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["HashedAccountId"]?.ToString()
            },
            useLegacyStyles: useLegacyStyles
            );
        }
    }
}