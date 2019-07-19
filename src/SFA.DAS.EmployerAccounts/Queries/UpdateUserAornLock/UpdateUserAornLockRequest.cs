using MediatR;
using System;

namespace SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock
{
    public class UpdateUserAornLockRequest : IAsyncRequest
    {
        public UpdateUserAornLockRequest(string userRef, bool success)
        {
            UserRef = Guid.Parse(userRef);
            Success = success;
        }

        public Guid UserRef { get; }
        public bool Success { get; }
    }
}