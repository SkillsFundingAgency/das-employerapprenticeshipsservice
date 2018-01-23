using System;
using System.Linq.Expressions;

namespace SFA.DAS.EAS.Domain
{
    public class DomainException<T> : DomainException where T : class
    {
        public DomainException(Expression<Func<T, object>> expression, string message)
            : base(expression, message)
        {
        }
    }

    public abstract class DomainException : Exception
    {
        public LambdaExpression Expression { get; protected set; }

        protected DomainException(LambdaExpression expression, string message)
            : base(message)
        {
            Expression = expression;
        }
    }
}