using MediatR;
using System;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock
{
    public class GetUserAornLockRequest : IAsyncRequest<GetUserAornLockResponse>
    {
        public GetUserAornLockRequest(string userRef)
        {
            this.userRef = Guid.Parse(userRef);
        }

        public Guid userRef { get; }
    }
}