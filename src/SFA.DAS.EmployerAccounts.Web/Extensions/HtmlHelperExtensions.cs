using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        private const string Draft = "DRAFT";
        private const string WithTrainingProvider = "WITH TRAINING PROVIDER";
        private const string ReadyForReview = "READY FOR REVIEW";
        private const string AddMoreDetails = "Add more details";
        private const string ViewApprenticeDetails = "View apprentice details";
        private const string ApproveOrRejectApprenticeDetails = "Approve or reject apprentice details";

        //"Add more details"
        //"View apprentice details"
        //"Approve or reject apprentice details"

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

        public static string GetCohortStatus(this HtmlHelper html, CohortStatus cohortStatus)
        {
            switch(cohortStatus)
            {
                case CohortStatus.Draft:
                    return Draft;
                case CohortStatus.WithTrainingProvider:
                    return WithTrainingProvider;
                case CohortStatus.Review:
                    return ReadyForReview;
                default:
                    return string.Empty;
            }
            //return cohortStatus switch
            //{
            //    CohortStatus.Draft => "DRAFT",
            //    CohortStatus.WithTrainingProvider => "WITH TRAINING PROVIDER",
            //    CohortStatus.Review => "READY FOR REVIEW",
            //    _ => string.Empty
            //};
        }

        public static string GetCallToActionButtonText(this HtmlHelper htmlHelper, CallToActionViewModel callToActionViewModel)
        {
            switch (callToActionViewModel.CohortStatus)
            {
                case (CohortStatus.Draft):
                    return AddMoreDetails;
                case (CohortStatus.WithTrainingProvider):
                    return callToActionViewModel.ViewApprenticeDetails ? ViewApprenticeDetails : AddMoreDetails;
                case (CohortStatus.Review):
                    return ApproveOrRejectApprenticeDetails;
                default:
                    return string.Empty;
            }
            //return callToActionViewModel.CohortStatus switch
            //{
            //    (CohortStatus.Draft) => "Add more details",
            //    (CohortStatus.WithTrainingProvider) => callToActionViewModel.ViewApprenticeDetails ? "View apprentice details" : "Add more details",
            //    (CohortStatus.Review) => "Approve or reject apprentice details"
            //    _ => string.Empty,
            //};
        }
    }
}