using MediatR;
using System;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock
{
    public class GetUserAornLockRequest : IAsyncRequest<GetUserAornLockResponse>
    {
        public GetUserAornLockRequest(string userRef)
        {
            UserRef = Guid.Parse(userRef);
        }

        public Guid UserRef { get; }
    }
}