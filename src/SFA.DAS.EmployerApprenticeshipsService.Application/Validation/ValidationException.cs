using System;

namespace SFA.DAS.EAS.Application.Validation
{
    public class ValidationException : Exception
    {
        public string PropertyName { get; }
        public string ErrorMessage { get; }

        public ValidationException(string message)
            : this(null, message)
        {
        }

        public ValidationException(string propertyName, string errorMessage)
            : base(errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}