using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommand : IAsyncRequest
    {
        public RemovePayeFromAccountCommand(string hashedAccountId, string payeRef, Guid externalUserId, bool removeScheme,string companyName)
        {
            HashedAccountId = hashedAccountId;
            PayeRef = payeRef;
            ExternalUserId = externalUserId;
            RemoveScheme = removeScheme;
            CompanyName = companyName;
        }

        public string HashedAccountId { get;  }
        public string PayeRef { get;  }
        public Guid ExternalUserId { get;  }
        public bool RemoveScheme { get; }

        public string CompanyName { get;  }
    }
}