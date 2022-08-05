using MediatR;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Helpers;
using SFA.DAS.EmployerFinance.Queries.GetContent;
using SFA.DAS.EmployerFinance.Web.App_Start;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Models.Links;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CdnLink(this HtmlHelper html, string folderName, string fileName)
        {
            var cdnLocation = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<EmployerFinanceConfiguration>().CdnBaseUrl;

            var trimCharacters = new char[] { '/' };
            return new MvcHtmlString($"{cdnLocation.Trim(trimCharacters)}/{folderName.Trim(trimCharacters)}/{fileName.Trim(trimCharacters)}");
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
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();            
            return configuration.ZenDeskSnippetKey;
        }

        public static string GetZenDeskSnippetSectionId(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            return configuration.ZenDeskSectionId;
        }

        public static string GetZenDeskCobrowsingSnippetKey(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            return configuration.ZenDeskCobrowsingSnippetKey;
        }

        public static IHeaderViewModel GetHeaderViewModel(this HtmlHelper html, bool useLegacyStyles = false)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();
            var employerFinanceBaseUrl = configuration.EmployerFinanceBaseUrl + (configuration.EmployerFinanceBaseUrl.EndsWith("/") ? "" : "/");
            var requestUrl = html.ViewContext.HttpContext.Request.Url;

            var headerModel = new HeaderViewModel(new HeaderConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl,
                ApplicationBaseUrl = configuration.EmployerAccountsBaseUrl,
                EmployerCommitmentsBaseUrl = configuration.EmployerCommitmentsBaseUrl,
                EmployerCommitmentsV2BaseUrl = configuration.EmployerCommitmentsV2BaseUrl,
                EmployerFinanceBaseUrl = configuration.EmployerFinanceBaseUrl,
                AuthenticationAuthorityUrl = configuration.Identity.BaseAddress,
                ClientId = configuration.Identity.ClientId,
                EmployerRecruitBaseUrl = configuration.EmployerRecruitBaseUrl,
                SignOutUrl = new Uri($"{employerFinanceBaseUrl}service/signOut"),
                ChangeEmailReturnUrl = requestUrl,
                ChangePasswordReturnUrl = requestUrl
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
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();

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

        public static ICookieBannerViewModel GetCookieBannerViewModel(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<EmployerFinanceConfiguration>();

            return new CookieBannerViewModel(new CookieBannerConfiguration
            {
                ManageApprenticeshipsBaseUrl = configuration.EmployerAccountsBaseUrl
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["accountHashedId"]?.ToString()
            }
            );
        }

        public static MvcHtmlString GetContentByType(this HtmlHelper html, string type, bool useLegacyStyles = false)
        {
            var mediator = DependencyResolver.Current.GetService<IMediator>();

            var response = AsyncHelper.RunSync(() => mediator.SendAsync(new GetContentRequest
            {
                UseLegacyStyles = useLegacyStyles,
                ContentType = type
            }));

            var content = response;
            return MvcHtmlString.Create(content.Content);
        }
    }
}   