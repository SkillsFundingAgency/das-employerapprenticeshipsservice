using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Application.Helpers
{
    public class NameOf<TModel> where TModel : class
    {
        public static string Property<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return ExpressionHelper.GetExpressionText(expression);
        }
    }

    public class NameOf
    {
        public static string Property<TModel, TProperty>(TModel obj, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            return ExpressionHelper.GetExpressionText(expression);
        }
    }
}