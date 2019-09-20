using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
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
        public static bool IsAuthorized(this HtmlHelper htmlHelper, string featureType)
        {
            var authorisationService = DependencyResolver.Current.GetService<IAuthorizationService>();
            var isAuthorized = authorisationService.IsAuthorized(featureType);

            return isAuthorized;
        }
    }
}