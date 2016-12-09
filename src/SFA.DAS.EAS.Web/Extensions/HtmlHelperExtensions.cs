using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using FluentValidation.Results;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString AddClassIfPropertyInError<TModel, TProperty>(
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

        public static MvcHtmlString AddClassIfPropertyIsMissingForApproval<TModel, TProperty>(
             this HtmlHelper<TModel> htmlHelper,
             Expression<Func<TModel, TProperty>> expression,
             string cssClass)
         {
             var expressionText = ExpressionHelper.GetExpressionText(expression);
             var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
             var state = htmlHelper.ViewData.ModelState;
 
             var approvalWarningState = htmlHelper.ViewBag.ApprovalWarningState as ValidationResult;
             var error = approvalWarningState?.Errors.FirstOrDefault(m => m.PropertyName == fullHtmlFieldName);
 
             if (error == null || !state.IsValid)
             {
                 return MvcHtmlString.Empty;
             }
 
             return new MvcHtmlString(cssClass);
         }
    }
}