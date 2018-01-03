using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SFA.DAS.Activities;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Activity(this HtmlHelper htmlHelper, ActivityType type, ActivityViewModel model)
        {
            return htmlHelper.Partial("Activities/_" + type, model);
        }

        public static bool ActivityViewExists(this HtmlHelper htmlHelper, ActivityType type)
        {
            var result = ViewEngines.Engines.FindView(htmlHelper.ViewContext, "Activities/_" + type, null);
            return result.View != null;
        }

        public static MvcHtmlString AddClassIfPropertyInError<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            string propertyName,
            string errorClass)
        {
            var state = htmlHelper.ViewData.ModelState[propertyName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(errorClass);
        }

        public static MvcHtmlString AddClassIfModelPropertyInError<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string errorClass)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(errorClass);
        }

        public static MvcHtmlString CommaSeperatedAddressToHtml(this HtmlHelper helper, string commaSeperatedAddress)
        {
            var htmlAddress = commaSeperatedAddress.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => $"{line.Trim()}<br/>")
                .Aggregate((x, y) => x + y);
            return new MvcHtmlString(htmlAddress);
        }
    }
}