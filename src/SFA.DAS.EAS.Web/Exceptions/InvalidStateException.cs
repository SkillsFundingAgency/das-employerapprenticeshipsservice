using System;

namespace SFA.DAS.EAS.Web.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message) { }
    }
}