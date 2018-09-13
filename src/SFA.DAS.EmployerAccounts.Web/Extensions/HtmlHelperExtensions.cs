using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CommaSeperatedAddressToHtml(this HtmlHelper htmlHelper, string commaSeperatedAddress)
        {
            var htmlAddress = commaSeperatedAddress.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => $"{line.Trim()}<br/>")
                .Aggregate("", (x, y) => x + y);

            return new MvcHtmlString(htmlAddress);
        }

        public static AuthorizationResult GetAuthorizationResult(this HtmlHelper htmlHelper, FeatureType featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var authorizationResult = authorisationService.GetAuthorizationResult(featureType);

            return authorizationResult;
        }

        public static bool IsAuthorized(this HtmlHelper htmlHelper, FeatureType featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }

        public static bool IsValid<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var partialFieldName = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(partialFieldName);

            if (htmlHelper.ViewData.ModelState.ContainsKey(fullHtmlFieldName))
            {
                var modelState = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
                var errors = modelState?.Errors;

                if (errors != null && errors.Any())
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ViewExists(this HtmlHelper html, string viewName)
        {
            var controllerContext = html.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, viewName, null);

            return result.View != null;
        }
    }
}
