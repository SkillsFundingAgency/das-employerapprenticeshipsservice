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
        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {
            var apiCallString =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!first) apiCallString += ",";
                first = false;

                apiCallString += $"'{label}'";
            }
            
            apiCallString += "] });</script>";

            return MvcHtmlString.Create(apiCallString);
        }
    }

    public static class ZenDeskLabels
    {
        public static string RegisterForAnApprenticeshipServiceAccount = "Register for an apprenticeship service account";
        public static string AddPAYESchemesToYourAccount = "Add PAYE schemes to your account";
        public static string ApprenticeshipFunding = "Apprenticeship funding";
        public static string EnteredTheWrongPAYESchemeDetails = "Entered the wrong PAYE scheme details";
        public static string UseYourGovernmentGatewayDetails = "Use your Government Gateway details";
        public static string ReviewTheEmployerAgreement = "Review the employer agreement";
    }
}