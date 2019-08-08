using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SFA.DAS.Validation
{
    public class ValidationException<T> : ValidationException
    {
        public ValidationException(Expression<Func<T, object>> expression, string message)
            : base(typeof(T), ExpressionHelper.GetExpressionText(expression), message)
        {
        }
    }

    public class ValidationException : Exception
    {
        public Type MessageType { get; protected set; }
        public string PropertyName { get; protected set; }

        public ValidationException(string message)
            : this(null, null, message)
        {
        }

        public ValidationException(Type messageType, string propertyName, string message)
            : base(message)
        {
            MessageType = messageType;
            PropertyName = propertyName;
        }
    }
}