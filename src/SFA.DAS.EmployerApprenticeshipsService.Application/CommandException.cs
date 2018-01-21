using System;
using System.Linq.Expressions;

namespace SFA.DAS.EAS.Application
{
    public class CommandException<T> : CommandException where T : class
    {
        public CommandException(Expression<Func<T, object>> expression, string message)
            : base(expression, message)
        {
        }
    }

    public abstract class CommandException : Exception
    {
        public LambdaExpression Expression { get; protected set; }

        protected CommandException(LambdaExpression expression, string message)
            : base(message)
        {
            Expression = expression;
        }
    }
}