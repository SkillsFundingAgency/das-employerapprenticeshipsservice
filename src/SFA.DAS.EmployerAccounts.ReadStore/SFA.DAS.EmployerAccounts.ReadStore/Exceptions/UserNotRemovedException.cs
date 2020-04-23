using System;

namespace SFA.DAS.EmployerAccounts.ReadStore.Exceptions
{
    [Serializable]
    public class UserNotRemovedException : Exception
    {
        public UserNotRemovedException() { }
        public UserNotRemovedException(string message) : base(message) { }
        public UserNotRemovedException(string message, Exception inner) : base(message, inner) { }
        protected UserNotRemovedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}