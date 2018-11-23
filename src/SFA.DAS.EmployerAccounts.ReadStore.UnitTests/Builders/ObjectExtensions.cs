using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Builders
{
    public static class ObjectExtensions
    {
        public static TObject SetPropertyTo<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> property, TProperty value)
        {
            var memberExp = (MemberExpression)property.Body;

            ((PropertyInfo)memberExp.Member).SetValue(obj, value);

            return obj;
        }
    }
}