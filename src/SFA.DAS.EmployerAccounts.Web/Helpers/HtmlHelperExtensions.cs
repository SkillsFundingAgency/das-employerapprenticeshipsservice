using System.Web.Mvc;
using SFA.DAS.Authorization.Results;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static AuthorizationResult GetAuthorizationResult(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authorizationResult = authorisationService.GetAuthorizationResult(featureType);

            return authorizationResult;
        }

        public static bool IsAuthorized(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }

        public static bool ViewExists(this HtmlHelper html, string viewName)
        {
            var controllerContext = html.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, viewName, null);

            return result.View != null;
        }

        public static MvcHtmlString SetZenDeskSuggestion(this HtmlHelper html, string suggestion)
        {
            return MvcHtmlString.Create($"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{suggestion}' }});</script>");
        }
    }

    public static class ZenDeskSuggestions
    {
        public static string RegisterForAnApprenticeshipServiceAccount = "Register for an apprenticeship service account";
        public static string AddPAYESchemesToYourAccount = "Add PAYE schemes to your account";
        public static string ApprenticeshipFunding = "Apprenticeship funding";
        public static string EnteredTheWrongPAYESchemeDetails = "Entered the wrong PAYE scheme details";
        public static string UseYourGovernmentGatewayDetails = "Use your Government Gateway details";
        public static string ReviewTheEmployerAgreement = "Review the employer agreement";
    }
}