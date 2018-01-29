using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;

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
            var dependencyResolver = DependencyResolver.Current;
            var currenUserService = dependencyResolver.GetService<ICurrentUserService>();
            var currentUser = currenUserService.GetCurrentUser();
            var featureToggleService = dependencyResolver.GetService<IFeatureToggleService>();

            return featureToggleService.IsFeatureEnabled(controllerName, actionName, currentUser.Email);
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