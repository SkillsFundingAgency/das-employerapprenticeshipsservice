using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authorization;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CommaSeperatedAddressToHtml(this HtmlHelper htmlHelper, string commaSeperatedAddress)
        {
            var htmlAddress = commaSeperatedAddress.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => $"{line.Trim()}<br/>")
                .Aggregate((x, y) => x + y);

            return new MvcHtmlString(htmlAddress);
        }

        public static bool IsFeatureEnabled(this HtmlHelper htmlHelper, string controllerName, string actionName)
        {
            var authorizationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var operationAuthorisationService = DependencyResolver.Current.GetService<IOperationAuthorisationService>();
            var authorizationContext = authorizationService.GetAuthorizationContext();
            var isFeatureEnabled = operationAuthorisationService.IsOperationAuthorised(authorizationContext);

            return isFeatureEnabled;
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
    }
}