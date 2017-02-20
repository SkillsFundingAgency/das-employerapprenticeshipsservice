using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
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
                .Select(line => $"<p>{line.Trim()}</p>")
                .Aggregate((x, y) => x + y);
            return new MvcHtmlString(htmlAddress);
        }
    }
}