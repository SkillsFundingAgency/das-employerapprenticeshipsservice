﻿using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : IAsyncRequest<GetLegalEntityResponse>
    {
        public GetLegalEntityQuery(string accountHashedId, long legalEntityId)
        {
            AccountHashedId = accountHashedId;
            LegalEntityId = legalEntityId;
        }

        public string AccountHashedId { get;  }

        public long LegalEntityId { get; }
    }
}